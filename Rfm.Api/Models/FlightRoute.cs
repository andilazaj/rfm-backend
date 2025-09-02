using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rfm.Api.Models;

public class FlightRoute
{
    [Key]
    public int Id { get; set; }

    public string Origin { get; set; } = default!;
    public string Destination { get; set; } = default!;


    public int? SeasonId { get; set; }

   
    [ForeignKey("SeasonId")]
    public Season? Season { get; set; }

    public List<BookingClass> BookingClasses { get; set; } = new();
}
