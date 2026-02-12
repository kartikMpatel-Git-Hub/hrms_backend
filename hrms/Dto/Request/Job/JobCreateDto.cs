using hrms.Model;

namespace hrms.Dto.Request.Job
{
    public class JobCreateDto
    {
        public string Title { get; set; }
        public string JobRole { get; set; }
        public string Place { get; set; }
        public string Requirements { get; set; }
        public IFormFile Jd {  get; set; }
        public int CreatedBy { get; set; }
        public int ContactTo { get; set; }
        public List<int> Reviewer {  get; set; } = new List<int>();
    }
}
