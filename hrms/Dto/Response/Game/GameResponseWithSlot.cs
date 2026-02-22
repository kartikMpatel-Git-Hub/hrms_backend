using hrms.Dto.Response.Game.GameSlot;
using hrms.Model;

namespace hrms.Dto.Response.Game
{
    public class GameResponseWithSlot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxPlayer { get; set; }
        public int MinPlayer { get; set; } = 0;
        public int Duration { get; set; } = 0;
        public List<GameOperationWindowResponseDto> GameOperationWindows { get; set; } = new List<GameOperationWindowResponseDto>();
    }
}
