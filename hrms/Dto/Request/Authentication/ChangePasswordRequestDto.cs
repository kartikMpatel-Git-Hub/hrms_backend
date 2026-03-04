namespace hrms.Dto.Request.Authentication
{
    public class ChangePasswordRequestDto
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}