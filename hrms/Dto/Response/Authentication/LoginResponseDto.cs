namespace hrms.Dto.Response.Authentication
{
    public class LoginResponseDto
    {
        public int id { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public string token { get; set; }
        public string image { get; set; }
    }
}
