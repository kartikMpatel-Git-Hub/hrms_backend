namespace hrms.Dto.Request.Game
{
    public class GameOperationWindowCreateDto
    {
        public TimeOnly OperationalStartTime { get; set; }
        public TimeOnly OperationalEndTime { get; set; }
    }
}