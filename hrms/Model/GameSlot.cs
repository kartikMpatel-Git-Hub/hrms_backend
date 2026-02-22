namespace hrms.Model
{
    public class GameSlot
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateTime Date { get; set; }
        public Game Game { get; set; }
        public int? BookedById { get; set; }
        public User? BookedBy { get; set; }
        public DateTime? BookedAt { get; set; }
        public GameSlotStatus Status { get; set; } = GameSlotStatus.AVAILABLE;
        public List<GameSlotPlayer> Players { get; set; } = new List<GameSlotPlayer>();

    }
}
