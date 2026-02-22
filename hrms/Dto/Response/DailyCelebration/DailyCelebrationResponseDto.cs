using hrms.Dto.Response.User;

namespace hrms.Dto.Response.DailyCelebration
{
    public class DailyCelebrationResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string EventType { get; set; }
        public DateTime EventDate { get; set; }
        public UserResponseDto User { get; set; }
    }
}