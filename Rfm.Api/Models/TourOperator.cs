using Rfm.Api.Infrastructure;
using Rfm.Api.Models;

public class TourOperator
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;

    public string IdentityUserId { get; set; } = null!; // ← Add this to link to AppUser

    public AppUser? User { get; set; }

    public ICollection<BookingClass> BookingClasses { get; set; } = new List<BookingClass>();
    public ICollection<TourOperatorSeason> TourOperatorSeasons { get; set; } = new List<TourOperatorSeason>();
}