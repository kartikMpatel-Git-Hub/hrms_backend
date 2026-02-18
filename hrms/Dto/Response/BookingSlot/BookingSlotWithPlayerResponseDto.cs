using hrms.Model;

namespace hrms.Dto.Response.BookingSlot
{
    public class BookingSlotWithPlayerResponseDto
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int BookedBy { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateTime Date { get; set; }
        public String Status { get; set; }
        public List<BookingPlayerResponseDto> Players { get; set; } = new List<BookingPlayerResponseDto>();
    }
}
