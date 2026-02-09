using hrms.Dto.Response.Traveler;

namespace hrms.Dto.Response.Travel
{
    public class TravelResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Desciption { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public int CreatedBy { get; set; }
        public decimal MaxAmountLimit { get; set; }
    }
}
