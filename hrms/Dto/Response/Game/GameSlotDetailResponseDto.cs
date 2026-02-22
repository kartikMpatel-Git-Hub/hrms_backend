using hrms.Dto.Response.User;
using hrms.Model;

namespace hrms.Dto.Response.Game
{
    public class GameSlotDetailResponseDto
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateTime Date { get; set; }
        public int? BookedById { get; set; }
        public UserMinimalResponseDto? BookedBy { get; set; }
        public DateTime? BookedAt { get; set; }
        public string Status { get; set; }
        public List<GameSlotPlayerResponseDto> Players { get; set; } = new List<GameSlotPlayerResponseDto>();
    }
}