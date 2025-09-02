namespace Rfm.Api.Models.Dtos
{
    public class SeasonDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Year { get; set; }
    }
}