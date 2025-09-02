using System.ComponentModel.DataAnnotations;

namespace Rfm.Api.Models;

public class BookingClass
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public List<FlightRoute> Routes { get; set; } = new();
}