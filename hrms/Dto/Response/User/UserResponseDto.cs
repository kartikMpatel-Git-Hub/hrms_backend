using hrms.Model;

namespace hrms.Dto.Response.User
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public String FullName { get; set; }
        public String Email { get; set; }
        public String Image { get; set; }
        public UserRole Role { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public DateOnly DateOfJoin { get; set; }
        public int? ManagerId { get; set; }
    }
}
