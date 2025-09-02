using Rfm.Api.Infrastructure;
using Rfm.Api.Models;

public class PriceEntry
{
    public int Id { get; set; }

    // Foreign Keys
    public int RouteId { get; set; }
    public FlightRoute Route { get; set; }

    public int SeasonId { get; set; }
    public Season Season { get; set; }

    public string TourOperatorId { get; set; }
    public AppUser TourOperator { get; set; }

    public int BookingClassId { get; set; }
    public BookingClass BookingClass { get; set; }

    // Pricing Info
    public DateTime Date { get; set; }

    // ✅ Now read-write
    public string DayOfWeek { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public int SeatCount { get; set; }
}