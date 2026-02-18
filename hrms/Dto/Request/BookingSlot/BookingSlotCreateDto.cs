using hrms.Model;

namespace hrms.Dto.Request.BookingSlot
{
    public class BookingSlotCreateDto
    {
        public int GameId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateTime Date { get; set; }
        public GameSlotStatus Status { get; set; }
    }
}
