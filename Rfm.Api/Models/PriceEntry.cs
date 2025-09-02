using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Rfm.Api.Infrastructure;

namespace Rfm.Api.Models
{
    public class PriceEntry
    {
        public int Id { get; set; }

        // Foreign keys
        [Required]
        public int RouteId { get; set; }
        public FlightRoute? Route { get; set; }

        [Required]
        public int SeasonId { get; set; }
        public Season? Season { get; set; }

        [Required]
        public string TourOperatorId { get; set; } = default!;
        public AppUser? TourOperator { get; set; }

        [Required]
        public int BookingClassId { get; set; }
        public BookingClass? BookingClass { get; set; }

        // Data
        [Required]
        public DateOnly Date { get; set; }   // EF Core 8+ maps this fine to SQLite/SQL Server

        [Precision(10, 2)]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int SeatCount { get; set; }
    }
}
