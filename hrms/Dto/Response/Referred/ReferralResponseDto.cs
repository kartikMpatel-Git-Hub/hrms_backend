using hrms.Dto.Response.Job;
using hrms.Dto.Response.User;
using hrms.Model;

namespace hrms.Dto.Response.Referred
{
    public class ReferralResponseDto
    {
        public int Id { get; set; }
        public string ReferedPersonName { get; set; }
        public string ReferedPersonEmail { get; set; }
        public string CvUrl { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public int ReferedBy { get; set; }
        public string Referer { get; set; }
        public int JobId { get; set; }
        public DateTime ReferedAt { get; set; }
    }
}
