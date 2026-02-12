namespace hrms.Model
{
    public class JobReviewer
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public Job Job { get; set; }
        public int ReviewerId { get; set; }
        public User Reviewer { get; set; }
    }
}
