namespace Rfm.Api.Models.Dtos
{
    public class TourOperatorDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public List<int> BookingClassIds { get; set; } = new();
        public List<int> SeasonIds { get; set; } = new();
    }
}