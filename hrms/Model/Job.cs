namespace hrms.Model
{
    public class Job : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string JobRole { get; set; }
        public string Place { get; set; }
        public string Requirements { get; set; }
        public string JdUrl { get; set; }
        public int CreatedBy { get; set; }
        public User Creater { get; set; }
        public int ContactTo { get; set; }
        public User Contact { get; set; }
        public bool IsActive { get; set; }
        public List<JobReviewer> Reviewers { get; set; } = new List<JobReviewer>();
    }
}
