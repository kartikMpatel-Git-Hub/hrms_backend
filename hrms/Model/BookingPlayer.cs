namespace hrms.Model
{
    public class BookingPlayer
    {
        public int Id { get; set; }
        public int SlotId { get; set; }
        public BookingSlot Slot { get; set; }
        public int PlayerId { get; set; }
        public User Player { get; set; }
        public bool is_deleted { get; set; }
    }
}
