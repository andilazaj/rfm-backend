namespace Rfm.Api.Models
{
    public class Season
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Year { get; set; }

        public List<TourOperatorSeason> TourOperatorSeasons { get; set; } = new();
    }
}