namespace hrms.Model
{
    public class GameOperationWindow : BaseEntity
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public TimeOnly OperationalStartTime { get; set; }
        public TimeOnly OperationalEndTime { get; set; }
        public Game Game { get; set; }

    }
}