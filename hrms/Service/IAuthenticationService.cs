using hrms.Dto.Request.Authentication;

namespace hrms.Service
{
    public interface IAuthenticationService
    {
        public Task RegisterNewUser(RegisterRequestDto dto);
        public Task<string> Login(LoginRequestDto dto);
    }
}
