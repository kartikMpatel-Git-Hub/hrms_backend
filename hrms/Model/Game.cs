namespace hrms.Model
{
    public class Game : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxPlayer { get; set; }
        public int Duration { get; set; }
        public int MinPlayer { get; set; }
        public int SlotAssignedBeforeMinutes  { get; set; }
        public int SlotCreateForNextXDays { get; set; }
        public List<GameOperationWindow> GameOperationWindows { get; set; } = new List<GameOperationWindow>();
    }
}
