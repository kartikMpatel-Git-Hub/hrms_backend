using hrms.Dto.Response.User;

namespace hrms.dto.response.System
{
    public class SystemSettingsResponseDto
    {
        public int Id { get; set; }
        public string? BirthdayImageUrl { get; set; }
        public string? AnniversaryImageUrl { get; set; }
        public string? DefaultProfileImageUrl { get; set; }
        public int? DefaultHrId { get; set; }
        public UserResponseDto? DefaultHr { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedById { get; set; }
        public UserResponseDto? UpdatedBy { get; set; }
    }
}