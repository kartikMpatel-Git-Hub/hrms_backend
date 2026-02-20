using hrms.Model;

namespace hrms.Dto.Response.User
{
    public class UserMinimalResponseDto
    {
        public int Id { get; set; }
        public String FullName { get; set; }
        public String Email { get; set; }
        public String Image { get; set; }
    }
}
