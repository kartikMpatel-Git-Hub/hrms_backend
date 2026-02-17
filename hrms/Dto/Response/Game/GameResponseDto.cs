namespace hrms.Dto.Response.Game
{
    public class GameResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxPlayer { get; set; }
        public int MinPlayer { get; set; } = 0;
    }
}
