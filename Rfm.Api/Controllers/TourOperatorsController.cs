using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rfm.Api.Infrastructure;
using Rfm.Api.Models;
using Rfm.Api.Models.Dtos;

namespace Rfm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TourOperatorsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public TourOperatorsController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var operators = await _context.TourOperators
            .Include(to => to.BookingClasses)
            .Include(to => to.TourOperatorSeasons)
                .ThenInclude(tos => tos.Season)
            .Include(to => to.User)
            .ToListAsync();

        var result = operators.Select(op => new
        {
            op.Id,
            op.Name,
            op.Email,
            Username = op.User?.UserName,
            Seasons = op.TourOperatorSeasons.Select(tos => new
            {
                tos.Season.Id,
                tos.Season.Name,
                tos.Season.Start,
                tos.Season.End,
                tos.Season.Year
            }),
            BookingClasses = op.BookingClasses.Select(bc => new
            {
                bc.Id,
                bc.Name
            })
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> Get(int id)
    {
        var op = await _context.TourOperators
            .Include(to => to.BookingClasses)
            .Include(to => to.TourOperatorSeasons)
                .ThenInclude(tos => tos.Season)
            .Include(to => to.User)
            .FirstOrDefaultAsync(to => to.Id == id);

        if (op == null) return NotFound();

        var result = new
        {
            op.Id,
            op.Name,
            op.Email,
            Username = op.User?.UserName,
            Seasons = op.TourOperatorSeasons.Select(tos => new
            {
                tos.Season.Id,
                tos.Season.Name,
                tos.Season.Start,
                tos.Season.End,
                tos.Season.Year
            }),
            BookingClasses = op.BookingClasses.Select(bc => new
            {
                bc.Id,
                bc.Name
            })
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TourOperatorDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return BadRequest(new { message = "Email is already registered." });

        var identityUser = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var createResult = await _userManager.CreateAsync(identityUser, dto.Password);
        if (!createResult.Succeeded)
        {
            return BadRequest(new { message = "Failed to create user", errors = createResult.Errors });
        }

        await _userManager.AddToRoleAsync(identityUser, "Operator");

        var op = new TourOperator
        {
            Name = dto.Name,
            Email = dto.Email,
            IdentityUserId = identityUser.Id
        };

  
        foreach (var id in dto.BookingClassIds)
        {
            var bc = await _context.BookingClasses.FindAsync(id);
            if (bc != null)
                op.BookingClasses.Add(bc);
        }

        
        foreach (var seasonId in dto.SeasonIds)
        {
            op.TourOperatorSeasons.Add(new TourOperatorSeason
            {
                SeasonId = seasonId
            });
        }

        _context.TourOperators.Add(op);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = op.Id }, new
        {
            op.Id,
            op.Name,
            op.Email
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TourOperatorDto dto)
    {
        var op = await _context.TourOperators
            .Include(o => o.BookingClasses)
            .Include(o => o.TourOperatorSeasons)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (op == null) return NotFound();

        op.Name = dto.Name;
        op.Email = dto.Email;

        
        op.BookingClasses.Clear();
        foreach (var idClass in dto.BookingClassIds)
        {
            var bc = await _context.BookingClasses.FindAsync(idClass);
            if (bc != null)
                op.BookingClasses.Add(bc);
        }

        
        op.TourOperatorSeasons.Clear();
        foreach (var seasonId in dto.SeasonIds)
        {
            op.TourOperatorSeasons.Add(new TourOperatorSeason
            {
                TourOperatorId = id,
                SeasonId = seasonId
            });
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var op = await _context.TourOperators
            .Include(o => o.BookingClasses)
            .Include(o => o.TourOperatorSeasons)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (op == null) return NotFound();

      
        if (op.User != null)
        {
            var userResult = await _userManager.DeleteAsync(op.User);
            if (!userResult.Succeeded)
            {
                return BadRequest(new { message = "Failed to delete associated user", errors = userResult.Errors });
            }

           
            return NoContent();
        }

        _context.TourOperatorSeasons.RemoveRange(op.TourOperatorSeasons);
        op.BookingClasses.Clear();
        _context.TourOperators.Remove(op);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
