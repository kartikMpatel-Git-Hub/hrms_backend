namespace hrms.Model
{
    public class GameSlotPlayer
    {
        public int Id { get; set; }
        public int SlotId { get; set; }
        public GameSlot GameSlot { get; set; }
        public int PlayerId { get; set; }
        public User Player { get; set; }
    }
}