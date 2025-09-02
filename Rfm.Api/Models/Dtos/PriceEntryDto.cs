public class PriceEntryDto
{
    public int RouteId { get; set; }
    public int SeasonId { get; set; }
    public string TourOperatorId { get; set; }
    public int BookingClassId { get; set; }
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public int SeatCount { get; set; }
}