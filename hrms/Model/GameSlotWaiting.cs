namespace hrms.Model
{
    public class GameSlotWaiting
    {
        public int Id { get; set; }
        public int GameSlotId { get; set; }
        public GameSlot GameSlot { get; set; }
        public int RequestedById { get; set; }
        public User RequestedBy { get; set; }
        public DateTime? RequestedAt { get; set; }
        public bool IsCancelled { get; set; } = false;
        public List<GameSlotWaitingPlayer> WaitingPlayers { get; set; } = new List<GameSlotWaitingPlayer>();
    }
}