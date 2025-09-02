using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rfm.Api.Infrastructure;
using Rfm.Api.Models;
using Rfm.Api.Models.Dtos;

namespace Rfm.Api.Controllers;

[ApiController]
[Route("api/routes")]
public class RoutesController : ControllerBase
{
    private readonly AppDbContext _db;

    public RoutesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRoute([FromBody] RouteDto dto)
    {
        var route = new FlightRoute
        {
            Origin = dto.Origin,
            Destination = dto.Destination,
            SeasonId = dto.SeasonId // ✅ assign season
        };

        var classes = await _db.BookingClasses
            .Where(c => dto.BookingClassIds.Contains(c.Id))
            .ToListAsync();

        route.BookingClasses = classes;

        _db.Routes.Add(route);
        await _db.SaveChangesAsync();

        var season = await _db.Seasons.FirstOrDefaultAsync(s => s.Id == route.SeasonId);

        return Ok(new
        {
            route.Id,
            route.Origin,
            route.Destination,
            Season = season != null ? new { season.Id, season.Name, season.Year } : null,
            BookingClasses = classes.Select(c => new { c.Id, c.Name }).ToList()
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoutes()
    {
        var routes = await _db.Routes
            .Include(r => r.BookingClasses)
            .Include(r => r.Season) // ✅ include season
            .Select(r => new
            {
                r.Id,
                r.Origin,
                r.Destination,
                Season = r.Season != null ? new { r.Season.Id, r.Season.Name, r.Season.Year } : null,
                BookingClasses = r.BookingClasses.Select(b => new { b.Id, b.Name })
            })
            .ToListAsync();

        return Ok(routes);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRoute(int id, [FromBody] RouteDto dto)
    {
        var route = await _db.Routes
            .Include(r => r.BookingClasses)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (route == null)
            return NotFound();

        route.Origin = dto.Origin;
        route.Destination = dto.Destination;
        route.SeasonId = dto.SeasonId;

        var bookingClasses = await _db.BookingClasses
            .Where(c => dto.BookingClassIds.Contains(c.Id))
            .ToListAsync();

        route.BookingClasses = bookingClasses;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Route updated successfully." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoute(int id)
    {
        var route = await _db.Routes.FindAsync(id);
        if (route == null)
            return NotFound(new { message = "Route not found." });

        _db.Routes.Remove(route);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Route deleted successfully." });
    }
}
