namespace Rfm.Api.Models
{
    public class TourOperatorSeason
    {
        public int TourOperatorId { get; set; }
        public TourOperator TourOperator { get; set; }

        public int SeasonId { get; set; }
        public Season Season { get; set; }
    }
}