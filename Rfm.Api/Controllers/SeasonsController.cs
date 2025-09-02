using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rfm.Api.Infrastructure;
using Rfm.Api.Models;
using Rfm.Api.Models.Dtos;

namespace Rfm.Api.Controllers;

[ApiController]
[Route("api/seasons")]
public class SeasonsController : ControllerBase
{
    private readonly AppDbContext _db;

    public SeasonsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SeasonDto dto)
    {
        var newSeason = new Season
        {
            Name = dto.Name,
            Start = dto.Start,
            End = dto.End,
            Year = dto.Year
        };

        _db.Seasons.Add(newSeason);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            newSeason.Id,
            newSeason.Name,
            newSeason.Start,
            newSeason.End,
            newSeason.Year
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var seasons = await _db.Seasons
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.Start,
                s.End,
                s.Year
            })
            .ToListAsync();

        return Ok(seasons);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SeasonDto dto)
    {
        var existingSeason = await _db.Seasons.FindAsync(id);
        if (existingSeason == null) return NotFound();

        existingSeason.Name = dto.Name;
        existingSeason.Start = dto.Start;
        existingSeason.End = dto.End;
        existingSeason.Year = dto.Year;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            existingSeason.Id,
            existingSeason.Name,
            existingSeason.Start,
            existingSeason.End,
            existingSeason.Year
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existingSeason = await _db.Seasons.FindAsync(id);
        if (existingSeason == null)
            return NotFound();

        var isUsed = await _db.Routes.AnyAsync(r => r.SeasonId == id);
        if (isUsed)
            return BadRequest("Cannot delete season because it is used in existing routes.");

        _db.Seasons.Remove(existingSeason);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Season deleted." });
    }
}
