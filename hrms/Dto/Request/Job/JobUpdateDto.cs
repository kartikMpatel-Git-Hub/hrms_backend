using hrms.Model;

namespace hrms.Dto.Request.Job
{
    public class JobUpdateDto
    {
        public string Title { get; set; }
        public string JobRole { get; set; }
        public string Place { get; set; }
        public string Requirements { get; set; }
        public bool IsActive { get; set; }
    }
}
