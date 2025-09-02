using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Rfm.Api.Infrastructure;
using Rfm.Api.Models;

namespace Rfm.Api.Controllers;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Email, string Role, string Token, string UserId, int? OperatorId);
public record RegisterRequest(string Email, string Password, string Role);

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IConfiguration _config;
    private readonly AppDbContext _context;

    public AuthController(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IConfiguration config,
        AppDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var existingUser = await _userManager.FindByEmailAsync(req.Email);
        if (existingUser != null)
            return BadRequest("User already exists.");

        if (!await _roleManager.RoleExistsAsync(req.Role))
            return BadRequest("Invalid role.");

        var user = new AppUser
        {
            UserName = req.Email,
            Email = req.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, req.Role);

        return Ok(new { message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, req.Password))
            return Unauthorized("Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Operator";

       
        int? operatorId = null;
        if (role == "Operator")
        {
            operatorId = await _context.TourOperators
                .Where(o => o.IdentityUserId == user.Id)
                .Select(o => (int?)o.Id)
                .FirstOrDefaultAsync();
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Email ?? ""),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new LoginResponse(
            Email: user.Email!,
            Role: role,
            Token: new JwtSecurityTokenHandler().WriteToken(token),
            UserId: user.Id,
            OperatorId: operatorId
        );
    }
}
