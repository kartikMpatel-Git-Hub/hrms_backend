using hrms.Dto.Response.User;

namespace hrms.Dto.Response.BookingSlot
{
    public class BookingPlayerResponseDto
    {
        public int Id { get; set; }
        public int SlotId { get; set; }
        public UserResponseDto Player { get; set; }
        public bool is_deleted { get; set; }
    }
}
