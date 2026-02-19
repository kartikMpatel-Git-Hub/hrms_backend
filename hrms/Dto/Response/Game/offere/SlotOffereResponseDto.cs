using hrms.Dto.Response.BookingSlot;
using hrms.Model;

namespace hrms.Dto.Response.Game.offere
{
    public class SlotOffereResponseDto
    {
        public int Id { get; set; }
        public BookingSlotResponseDto Slot {get; set;}
        public int OffereTo {  get; set; }
        public int PriorityOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public String Status { get; set; }
    }
}
