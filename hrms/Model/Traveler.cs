namespace hrms.Model
{
    public class Traveler
    {
        public int Id { get; set; }
        public int TravelId { get; set; }
        public Travel Travel { get; set; }
        public int TravelerId { get; set; }
        public User Travelerr { get; set; }
        public bool is_deletd { get; set; }
    }
}
