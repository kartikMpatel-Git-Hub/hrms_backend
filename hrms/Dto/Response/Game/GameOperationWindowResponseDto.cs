namespace hrms.Dto.Response.Game
{
    public class GameOperationWindowResponseDto
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public TimeOnly OperationalStartTime { get; set; }
        public TimeOnly OperationalEndTime { get; set; }
    }
}