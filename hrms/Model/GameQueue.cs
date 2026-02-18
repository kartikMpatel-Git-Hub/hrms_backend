namespace hrms.Model
{
    public class GameQueue
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
        public int PlayerId { get; set; }
        public User Player { get; set; }
        public bool is_deleted { get; set; }
        public GameQueueStatus Status { get; set; }
        public DateTime EnqueueAt { get; set; }
    }

    public enum GameQueueStatus
    {
        INQUEUE,
        COMPLETED
    }
}
