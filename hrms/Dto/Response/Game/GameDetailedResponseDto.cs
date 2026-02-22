namespace hrms.Dto.Response.Game
{
    public class GameDetailedResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxPlayer { get; set; }
        public int MinPlayer { get; set; } = 0;
        public int SlotAssigedBeforeXMinutes { get; set; } = 30;
        public int SlotCreateForNextXDays { get; set; } = 7;
        public int Duration { get; set; } = 0;
        public List<GameOperationWindowResponseDto> GameOperationWindows { get; set; } = new List<GameOperationWindowResponseDto>();
    }
}
