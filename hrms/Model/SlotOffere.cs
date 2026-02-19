namespace hrms.Model
{
    public class SlotOffere
    {
        public int Id { get; set; }
        public int BookingSlotId { get; set; }
        public BookingSlot Slot { get; set; }
        public int OffereTo {  get; set; }
        public User Offered { get; set; }
        public int PriorityOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public SlotOfferStatus Status { get; set; }
    }

    public enum SlotOfferStatus
    {
        InProcess,
        Pending,
        Accepted,
        Expired
    }
}
