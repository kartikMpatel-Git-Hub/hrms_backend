using hrms.Dto.Request.Authentication;
using hrms.Dto.Response.Authentication;
using hrms.Dto.Response.User;
using hrms.Service;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace hrms.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EnableCors("MyAllowSpecificOrigins")]
    public class AuthenticationController : ControllerBase
    {
        
        private readonly IAuthenticationService _service;

        public AuthenticationController(IAuthenticationService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            UserResponseDto response = await _service.RegisterNewUser(dto);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            Console.WriteLine("HERE");
            LoginResponseDto response = await _service.Login(dto);
            Response.Cookies.Append("jwt_token", response.token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(60)
            });
            return Ok(response);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_token");
            return Ok(new { message = "Logged out successfully" });
        }

    }
}
