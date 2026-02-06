using hrms.Dto.Request.Authentication;
using hrms.Service;
using Microsoft.AspNetCore.Mvc;

namespace hrms.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        
        private readonly IAuthenticationService _service;

        public AuthenticationController(IAuthenticationService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {

            await _service.RegisterNewUser(dto);
            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            Console.WriteLine("HERE");
            string generatedToken = await _service.Login(dto);
            Response.Cookies.Append("jwt_token", generatedToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(60)
            });
            return Ok(new { token = generatedToken });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_token");
            return Ok(new { message = "Logged out successfully" });
        }

    }
}
