namespace hrms.Model
{
    public class JobShared
    {
        public int Id { get; set; }
        public string SharedTo { get; set; }
        public int SharedBy { get; set; }
        public User Shared {  get; set; }
        public int JobId { get; set; }
        public Job Job  { get; set; }
        public bool is_deleted { get; set; }
        public DateTime SharedAt { get; set; }
    }
}
