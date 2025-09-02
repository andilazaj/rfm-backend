using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rfm.Api.Models;
using Rfm.Api.Infrastructure;

[ApiController]
[Route("api/[controller]")]
public class PriceEntriesController : ControllerBase
{
    private readonly AppDbContext _db;

    public PriceEntriesController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/priceentries
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var prices = await _db.PriceEntries
            .Include(p => p.Route)
            .Include(p => p.Season)
            .Include(p => p.TourOperator)
            .ToListAsync();

        return Ok(prices);
    }

    // GET: api/priceentries/operator/{operatorId}
    [HttpGet("operator/{operatorId}")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<IActionResult> GetByOperator(string operatorId)
    {
        var prices = await _db.PriceEntries
            .Where(p => p.TourOperatorId == operatorId)
            .Include(p => p.Route)
            .Include(p => p.Season)
            .ToListAsync();

        return Ok(prices);
    }

    // POST: api/priceentries
    [HttpPost]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<IActionResult> Create([FromBody] PriceEntry entry)
    {
        _db.PriceEntries.Add(entry);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
    }

    // GET: api/priceentries/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<IActionResult> GetById(int id)
    {
        var entry = await _db.PriceEntries
            .Include(p => p.Route)
            .Include(p => p.Season)
            .Include(p => p.TourOperator)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (entry == null) return NotFound();
        return Ok(entry);
    }

    // PUT: api/priceentries/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<IActionResult> Update(int id, [FromBody] PriceEntry updated)
    {
        var existing = await _db.PriceEntries.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Date = updated.Date;
        existing.DayOfWeek = updated.DayOfWeek;
        existing.RouteId = updated.RouteId;
        existing.SeasonId = updated.SeasonId;
        existing.TourOperatorId = updated.TourOperatorId;
        existing.BookingClassId = updated.BookingClassId;
        existing.Price = updated.Price;
        existing.SeatCount = updated.SeatCount;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/priceentries/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entry = await _db.PriceEntries.FindAsync(id);
        if (entry == null) return NotFound();

        _db.PriceEntries.Remove(entry);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
