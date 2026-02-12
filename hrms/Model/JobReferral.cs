namespace hrms.Model
{
    public class JobReferral
    {
        public int Id { get; set; }
        public string ReferedPersonName { get; set; }
        public string ReferedPersonEmail { get; set; }
        public string CvUrl { get; set; }
        public string Note { get; set; }
        public int ReferedBy { get; set; }
        public User Referer { get; set; }
        public int JobId { get; set; }
        public Job Job { get; set; }
        public bool is_deleted { get; set; }
        public DateTime ReferedAt { get; set; }
    }
}
