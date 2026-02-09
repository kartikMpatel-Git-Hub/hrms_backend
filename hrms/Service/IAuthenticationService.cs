using hrms.Dto.Request.Authentication;
using hrms.Dto.Response.User;

namespace hrms.Service
{
    public interface IAuthenticationService
    {
        public Task<UserResponseDto> RegisterNewUser(RegisterRequestDto dto);
        public Task<string> Login(LoginRequestDto dto);
    }
}
