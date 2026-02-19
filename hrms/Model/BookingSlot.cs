namespace hrms.Model
{
    public class BookingSlot : BaseEntity
    {
        public int Id { get; set;  }
        public int GameId { get; set; }
        public Game? Game { get; set; }
        public int? BookedBy { get; set; }
        public User? Booked {  get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateTime Date { get; set; }
        public GameSlotStatus Status { get; set; }
        public int? RequestBy { get; set; }
        public User? Requester { get; set; }
        public int CurrentPriorityOrder { get; set; } = 0;
        public List<BookingPlayer> Players { get; set; } = new List<BookingPlayer>();
        public List<SlotOffere> Offers { get; set; } = new List<SlotOffere>();
        public List<RequestedPlayer> RequestedPlayers { get; set; } = new List<RequestedPlayer>();
    }

    public enum GameSlotStatus
    {
        AVAILABLE,
        BOOKED,
        WAITING
    }
}
