namespace hrms.Dto.Response.Game.GameSlot
{
    public class GameSlotResponseDto
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
