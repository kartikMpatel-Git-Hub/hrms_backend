namespace hrms.Dto.Response.Job
{
    public class JobResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string JobRole { get; set; }
        public string Place { get; set; }
        public string Requirements { get; set; }
        public string JdUrl { get; set; }
        public int CreatedBy { get; set; }
        public int ContactTo { get; set; }
        public bool IsActive { get; set; }
    }
}
