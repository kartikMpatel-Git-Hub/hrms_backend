namespace hrms.Model
{
    public class GameSlotWaitingPlayer
    {
        public int Id { get; set; }
        public int GameSlotWaitingId { get; set; }
        public GameSlotWaiting GameSlotWaiting { get; set; }
        public int PlayerId { get; set; }
        public User Player { get; set; }
    }
}