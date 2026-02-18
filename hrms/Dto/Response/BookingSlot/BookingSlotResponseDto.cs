using hrms.Model;

namespace hrms.Dto.Response.BookingSlot
{
    public class BookingSlotResponseDto
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int BookedBy { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateTime Date { get; set; }
        public String Status { get; set; }
    }
}
