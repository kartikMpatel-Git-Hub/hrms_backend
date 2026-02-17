namespace hrms.Model
{
    public class Game : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxPlayer { get; set; }
        public int MinPlayer { get; set; }
        public List<GameSlot> Slots { get; set; } = new List<GameSlot>();
    }
}
