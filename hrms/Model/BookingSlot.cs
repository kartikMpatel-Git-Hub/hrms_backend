namespace hrms.Model
{
    public class BookingSlot : BaseEntity
    {
        public int Id { get; set;  }
        public int GameId { get; set; }
        public Game Game { get; set; }
        public int? BookedBy { get; set; }
        public User? Booked {  get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateTime Date { get; set; }
        public GameSlotStatus Status { get; set; }
        public List<BookingPlayer> Players { get; set; } = new List<BookingPlayer>();
    }

    public enum GameSlotStatus
    {
        AVAILABLE,
        BOOKED,
        COMPLETED
    }
}
