using hrms.Dto.Response.User;
using hrms.Model;

namespace hrms.Dto.Response.Game
{
    public class UpcomingBookingSlotResponseDto
    {
        public int Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateTime Date { get; set; }
        public GameResponseDto Game { get; set; }
        public UserMinimalResponseDto BookedBy { get; set; }
        public DateTime BookedAt { get; set; }
        public String Status { get; set; }
        public int PlayerCount { get; set; } = 0;
    }
}