using hrms.Dto.Request.Game;
using hrms.Dto.Response.User;
using hrms.Model;

namespace hrms.Dto.Response.Game
{
    public class GameSlotWaitinglistResponseDto
    {
        public int Id { get; set; }
        public int GameSlotId { get; set; }
        public int RequestedById { get; set; }
        public DateTime? RequestedAt { get; set; }
        public UserMinimalResponseDto RequestedBy { get; set; }
        public List<WaitlistPlayerResponseDto> WaitingPlayers { get; set; } = new List<WaitlistPlayerResponseDto>();
    }
}