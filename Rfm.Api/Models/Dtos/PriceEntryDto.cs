namespace Rfm.Api.Dtos;  

public class PriceEntryCreateDto
{
    public int RouteId { get; set; }
    public int SeasonId { get; set; }
    public string TourOperatorId { get; set; } = default!;
    public int BookingClassId { get; set; }
    public DateOnly Date { get; set; }
    public decimal Price { get; set; }
    public int SeatCount { get; set; }
}

public class PriceEntryReadDto
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string DayOfWeek { get; set; } = default!;
    public decimal Price { get; set; }
    public int SeatCount { get; set; }

    public int RouteId { get; set; }
    public int SeasonId { get; set; }
    public string TourOperatorId { get; set; } = default!;
    public int BookingClassId { get; set; }

    // Optional display fields
    public string? RouteName { get; set; }
    public string? BookingClassName { get; set; }
    public string? OperatorName { get; set; }
}

public class PriceEntryUpsertDto
{
    public int RouteId { get; set; }
    public int SeasonId { get; set; }
    public string TourOperatorId { get; set; } = default!;
    public int BookingClassId { get; set; }
    public DateOnly Date { get; set; }
    public decimal Price { get; set; }
    public int SeatCount { get; set; }
}
