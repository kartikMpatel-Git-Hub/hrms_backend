using hrms.Dto.Response.Game.GameSlot;

namespace hrms.Dto.Response.Game
{
    public class GameResponseWithSlot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxPlayer { get; set; }
        public int MinPlayer { get; set; } = 0;
        public List<GameSlotResponseDto> Slots { get; set; } = new List<GameSlotResponseDto>();
    }
}
