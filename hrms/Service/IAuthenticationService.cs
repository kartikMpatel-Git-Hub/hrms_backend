using hrms.Dto.Request.Authentication;
using hrms.Dto.Response.Authentication;
using hrms.Dto.Response.User;

namespace hrms.Service
{
    public interface IAuthenticationService
    {
        public Task<UserResponseDto> RegisterNewUser(RegisterRequestDto dto);
        public Task<LoginResponseDto> Login(LoginRequestDto dto);
        Task ForgetPassword(ForgetPasswordRequestDto dto);
    }
}
