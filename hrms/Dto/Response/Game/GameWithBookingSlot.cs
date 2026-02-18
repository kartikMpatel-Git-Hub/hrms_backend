using hrms.Dto.Response.BookingSlot;

namespace hrms.Dto.Response.Game
{
    public class GameWithBookingSlot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxPlayer { get; set; }
        public int MinPlayer { get; set; } = 0;
        public List<BookingSlotResponseDto> BookingSlots { get; set; } = new List<BookingSlotResponseDto>();

    }
}
