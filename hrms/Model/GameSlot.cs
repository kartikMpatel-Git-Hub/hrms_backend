namespace hrms.Model
{
    public class GameSlot : BaseEntity
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
