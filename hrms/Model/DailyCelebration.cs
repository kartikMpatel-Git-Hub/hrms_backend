namespace hrms.Model
{
    public class DailyCelebration
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public EventType EventType { get; set; }
        public DateTime EventDate { get; set; }
    }
    
    public enum EventType
    {
        Birthday,
        WorkAnniversary
    }
}
