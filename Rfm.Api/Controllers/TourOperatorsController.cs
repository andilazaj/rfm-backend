using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rfm.Api.Models;
using Rfm.Api.Models.Dtos;

namespace Rfm.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TourOperatorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TourOperatorsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            var operators = await _context.TourOperators
                .Include(to => to.BookingClasses)
                    .ThenInclude(bc => bc.Routes)
                        .ThenInclude(r => r.Season)
                .Include(to => to.TourOperatorSeasons)
                    .ThenInclude(tos => tos.Season)
                .ToListAsync();

            var result = operators.Select(op => new
            {
                op.Id,
                op.Name,
                op.Email,
                Seasons = op.TourOperatorSeasons.Select(tos => new
                {
                    tos.Season.Id,
                    tos.Season.Name,
                    tos.Season.Start,
                    tos.Season.End,
                    tos.Season.Year
                }).ToList(),
                BookingClasses = op.BookingClasses.Select(bc => new
                {
                    bc.Id,
                    bc.Name,
                    Routes = bc.Routes
                        .Where(r => r.BookingClasses.Any(x => x.Id == bc.Id))
                        .Select(r => new
                        {
                            r.Id,
                            r.Origin,
                            r.Destination,
                            r.SeasonId,
                            Season = new
                            {
                                r.Season.Id,
                                r.Season.Name,
                                r.Season.Start,
                                r.Season.End,
                                r.Season.Year
                            },
                            BookingClasses = r.BookingClasses.Select(b => b.Name).ToList()
                        }).ToList()
                }).ToList()
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> Get(int id)
        {
            var op = await _context.TourOperators
                .Include(to => to.BookingClasses)
                    .ThenInclude(bc => bc.Routes)
                        .ThenInclude(r => r.Season)
                .Include(to => to.TourOperatorSeasons)
                    .ThenInclude(tos => tos.Season)
                .FirstOrDefaultAsync(to => to.Id == id);

            if (op == null) return NotFound();

            var result = new
            {
                op.Id,
                op.Name,
                op.Email,
                Seasons = op.TourOperatorSeasons.Select(tos => new
                {
                    tos.Season.Id,
                    tos.Season.Name,
                    tos.Season.Start,
                    tos.Season.End,
                    tos.Season.Year
                }).ToList(),
                BookingClasses = op.BookingClasses.Select(bc => new
                {
                    bc.Id,
                    bc.Name,
                    Routes = bc.Routes
                        .Where(r => r.BookingClasses.Any(x => x.Id == bc.Id))
                        .Select(r => new
                        {
                            r.Id,
                            r.Origin,
                            r.Destination,
                            r.SeasonId,
                            Season = new
                            {
                                r.Season.Id,
                                r.Season.Name,
                                r.Season.Start,
                                r.Season.End,
                                r.Season.Year
                            },
                            BookingClasses = r.BookingClasses.Select(b => b.Name).ToList()
                        }).ToList()
                }).ToList()
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TourOperatorDto dto)
        {
            var bookingClasses = await _context.BookingClasses
                .Where(bc => dto.BookingClassIds.Contains(bc.Id))
                .ToListAsync();

            var seasons = await _context.Seasons
                .Where(s => dto.SeasonIds.Contains(s.Id))
                .ToListAsync();

            var op = new TourOperator
            {
                Name = dto.Name,
                Email = dto.Email,
                BookingClasses = bookingClasses,
                TourOperatorSeasons = seasons.Select(s => new TourOperatorSeason
                {
                    SeasonId = s.Id
                }).ToList()
            };

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

            var bookingClasses = await _context.BookingClasses
                .Where(bc => dto.BookingClassIds.Contains(bc.Id))
                .ToListAsync();

            var seasons = await _context.Seasons
                .Where(s => dto.SeasonIds.Contains(s.Id))
                .ToListAsync();

            op.BookingClasses = bookingClasses;
            op.TourOperatorSeasons = seasons.Select(s => new TourOperatorSeason
            {
                TourOperatorId = id,
                SeasonId = s.Id
            }).ToList();

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var op = await _context.TourOperators
                .Include(o => o.BookingClasses)
                .Include(o => o.TourOperatorSeasons)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (op == null) return NotFound();

            // Clear BookingClass relations
            op.BookingClasses.Clear();

            // Remove TourOperatorSeason links
            _context.TourOperatorSeasons.RemoveRange(op.TourOperatorSeasons);

            _context.TourOperators.Remove(op);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
