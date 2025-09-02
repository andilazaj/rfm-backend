namespace Rfm.Api.Models
{
    public class TourOperator
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public List<BookingClass> BookingClasses { get; set; } = new();
        public List<TourOperatorSeason> TourOperatorSeasons { get; set; } = new();
    }
}