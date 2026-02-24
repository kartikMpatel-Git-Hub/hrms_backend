using hrms.Dto.Response.Traveler;

namespace hrms.Dto.Response.Travel
{
    public class TravelWithTravelerResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public int CreatedBy { get; set; }
        public decimal MaxAmountLimit { get; set; }
        public List<TravelerDto> Travelers { get; set; } = new List<TravelerDto>();
    }
}
