namespace Rfm.Api.Models.Dtos;

public class RouteDto
{
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public List<int> BookingClassIds { get; set; } = new();
    public int? SeasonId { get; set; }
}