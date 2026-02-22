namespace hrms.Dto.Response.Game
{
    public class GameResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxPlayer { get; set; }
        public int MinPlayer { get; set; } = 0;
        public int Duration { get; set; } = 0;
        public int SlotAssignedBeforeMinutes  { get; set; }
        public int SlotCreateForNextXDays { get; set; }
    }
}
