using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rfm.Api.Auth;   
using Rfm.Api.Dtos;     
using Rfm.Api.Infrastructure;
using Rfm.Api.Models;

namespace Rfm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PriceEntriesController : ControllerBase
{
    private readonly AppDbContext _db;

    public PriceEntriesController(AppDbContext db) => _db = db;

    
    [HttpGet]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<IActionResult> Query(
        [FromQuery] string? operatorId,
        [FromQuery] int? routeId,
        [FromQuery] int? seasonId,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 200)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 1000) pageSize = 200;

        
        if (!HttpContext.IsAdmin())
        {
            var me = HttpContext.GetUserId();
            if (string.IsNullOrEmpty(me)) return Forbid();
            operatorId = me;
        }

        var q = _db.PriceEntries.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(operatorId))
            q = q.Where(p => p.TourOperatorId == operatorId);
        if (routeId.HasValue)
            q = q.Where(p => p.RouteId == routeId);
        if (seasonId.HasValue)
            q = q.Where(p => p.SeasonId == seasonId);
        if (from.HasValue)
            q = q.Where(p => p.Date >= from.Value);
        if (to.HasValue)
            q = q.Where(p => p.Date <= to.Value);

        var total = await q.CountAsync();

        var items = await q
            .OrderBy(p => p.Date)
            .ThenBy(p => p.BookingClassId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PriceEntryReadDto
            {
                Id = p.Id,
                Date = p.Date,
                DayOfWeek = p.Date.ToDateTime(TimeOnly.MinValue).DayOfWeek.ToString(),
                Price = p.Price,
                SeatCount = p.SeatCount,
                RouteId = p.RouteId,
                SeasonId = p.SeasonId,
                TourOperatorId = p.TourOperatorId,
                BookingClassId = p.BookingClassId
            })
            .ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }

    
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _db.PriceEntries
            .AsNoTracking()
            .Include(x => x.Route)
            .Include(x => x.Season)
            .Include(x => x.TourOperator)
            .Include(x => x.BookingClass)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (p is null) return NotFound();

        
        if (!HttpContext.IsAdmin())
        {
            var me = HttpContext.GetUserId();
            if (!string.Equals(me, p.TourOperatorId, StringComparison.Ordinal))
                return Forbid();
        }

        var dto = new PriceEntryReadDto
        {
            Id = p.Id,
            Date = p.Date,
            DayOfWeek = p.Date.ToDateTime(TimeOnly.MinValue).DayOfWeek.ToString(),
            Price = p.Price,
            SeatCount = p.SeatCount,
            RouteId = p.RouteId,
            SeasonId = p.SeasonId,
            TourOperatorId = p.TourOperatorId,
            BookingClassId = p.BookingClassId,
            RouteName = p.Route != null ? $"{p.Route.Origin} → {p.Route.Destination}" : null,
            BookingClassName = p.BookingClass?.Name,
            OperatorName = p.TourOperator?.UserName
        };

        return Ok(dto);
    }

   
    [HttpPost]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<IActionResult> Create([FromBody] PriceEntryCreateDto dto)
    {
       
        if (!HttpContext.IsAdmin())
        {
            var me = HttpContext.GetUserId();
            if (string.IsNullOrEmpty(me) || !string.Equals(me, dto.TourOperatorId, StringComparison.Ordinal))
                return Forbid();
        }

        var entity = new PriceEntry
        {
            RouteId = dto.RouteId,
            SeasonId = dto.SeasonId,
            TourOperatorId = dto.TourOperatorId,
            BookingClassId = dto.BookingClassId,
            Date = dto.Date,
            Price = dto.Price,
            SeatCount = dto.SeatCount
        };

        _db.PriceEntries.Add(entity);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            return Conflict(new { message = "A price entry for this Operator/Route/Season/Class/Date already exists." });
        }

        var read = new PriceEntryReadDto
        {
            Id = entity.Id,
            Date = entity.Date,
            DayOfWeek = entity.Date.ToDateTime(TimeOnly.MinValue).DayOfWeek.ToString(),
            Price = entity.Price,
            SeatCount = entity.SeatCount,
            RouteId = entity.RouteId,
            SeasonId = entity.SeasonId,
            TourOperatorId = entity.TourOperatorId,
            BookingClassId = entity.BookingClassId
        };

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, read);
    }

   
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<IActionResult> Update(int id, [FromBody] PriceEntryCreateDto dto)
    {
        var entity = await _db.PriceEntries.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();

        if (!HttpContext.IsAdmin())
        {
            var me = HttpContext.GetUserId();
            if (string.IsNullOrEmpty(me) || !string.Equals(me, entity.TourOperatorId, StringComparison.Ordinal))
                return Forbid();
        }

        entity.RouteId = dto.RouteId;
        entity.SeasonId = dto.SeasonId;
        entity.TourOperatorId = dto.TourOperatorId; 
        entity.BookingClassId = dto.BookingClassId;
        entity.Date = dto.Date;
        entity.Price = dto.Price;
        entity.SeatCount = dto.SeatCount;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            return Conflict(new { message = "Unique constraint conflict on (Operator, Route, Season, Class, Date)." });
        }

        return NoContent();
    }

 
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.PriceEntries.FindAsync(id);
        if (entity is null) return NotFound();
        _db.PriceEntries.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }

   
    [HttpPost("bulk-upsert")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<IActionResult> BulkUpsert([FromBody] List<PriceEntryUpsertDto> rows)
    {
        if (rows is null || rows.Count == 0) return BadRequest("No rows provided.");

        if (!HttpContext.IsAdmin())
        {
            var me = HttpContext.GetUserId();
            if (string.IsNullOrEmpty(me)) return Forbid();
            if (rows.Any(r => !string.Equals(r.TourOperatorId, me, StringComparison.Ordinal)))
                return Forbid();
        }

        
        var opIds = rows.Select(r => r.TourOperatorId).Distinct().ToList();
        var routeIds = rows.Select(r => r.RouteId).Distinct().ToList();
        var seasonIds = rows.Select(r => r.SeasonId).Distinct().ToList();
        var classIds = rows.Select(r => r.BookingClassId).Distinct().ToList();
        var dates = rows.Select(r => r.Date).Distinct().ToList();

        var existing = await _db.PriceEntries
            .Where(p => opIds.Contains(p.TourOperatorId)
                     && routeIds.Contains(p.RouteId)
                     && seasonIds.Contains(p.SeasonId)
                     && classIds.Contains(p.BookingClassId)
                     && dates.Contains(p.Date))
            .ToListAsync();

        var dict = existing.ToDictionary(
            e => (e.TourOperatorId, e.RouteId, e.SeasonId, e.BookingClassId, e.Date));

        foreach (var r in rows)
        {
            var key = (r.TourOperatorId, r.RouteId, r.SeasonId, r.BookingClassId, r.Date);
            if (dict.TryGetValue(key, out var e))
            {
                e.Price = r.Price;
                e.SeatCount = r.SeatCount;
            }
            else
            {
                _db.PriceEntries.Add(new PriceEntry
                {
                    TourOperatorId = r.TourOperatorId,
                    RouteId = r.RouteId,
                    SeasonId = r.SeasonId,
                    BookingClassId = r.BookingClassId,
                    Date = r.Date,
                    Price = r.Price,
                    SeatCount = r.SeatCount
                });
            }
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        => ex.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true
        || ex.Message.Contains("unique", StringComparison.OrdinalIgnoreCase);
}
