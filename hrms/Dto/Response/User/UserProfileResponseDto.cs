using hrms.dto.response.user;
using hrms.Dto.Request.User;
using hrms.Dto.Response.Department;

namespace hrms.Dto.Response.User
{
    public class UserProfileResponseDto
    {
        public int Id { get; set; }
        public String FullName { get; set; }
        public String Email { get; set; }
        public String Image { get; set; }
        public String Role { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfJoin { get; set; }
        public int? ReportTo { get; set; }
        public UserResponseDto Reported { get; set; }
        public DepartmentResponseDto Department { get; set; }
        public string Designation { get; set; }
        public List<UserResponseDto> Team { get; set; } = new List<UserResponseDto>();
    }
}