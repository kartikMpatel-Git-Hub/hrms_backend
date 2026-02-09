using hrms.Model;

namespace hrms.Dto.Response.User
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public String FullName { get; set; }
        public String Email { get; set; }
        public String Image { get; set; }
        public String Role { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfJoin { get; set; }
        public int? ManagerId { get; set; }
    }
}
