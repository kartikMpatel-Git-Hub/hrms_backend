using hrms.Model;

namespace hrms.Dto.Response.Game
{
    public class GameSlotResponseDto
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateTime Date { get; set; }
        public DateTime? BookedAt { get; set; }
        public string Status { get; set; }
    }
}