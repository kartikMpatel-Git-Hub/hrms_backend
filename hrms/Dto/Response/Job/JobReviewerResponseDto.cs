using hrms.Dto.Response.User;

namespace hrms.Dto.Response.Job
{
    public class JobReviewerResponseDto
    {
        public int Id { get; set; }
        public UserResponseDto Reviewer { get; set; }
    }
}
