namespace hrms.Model
{
    public class UserGameState
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int GamePlayed { get; set; }
        public DateTime LastPlayedAt { get; set; }
    }
}
