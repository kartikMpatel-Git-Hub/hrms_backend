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
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IAuthenticationService service, ILogger<AuthenticationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("register")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            UserResponseDto response = await _service.RegisterNewUser(dto);
            _logger.LogInformation("[{Method}] {Url} - User registered successfully", Request.Method, Request.Path);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            LoginResponseDto response = await _service.Login(dto);
            Response.Cookies.Append("jwt_token", response.token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(60)
            });
            _logger.LogInformation("[{Method}] {Url} - User logged in successfully", Request.Method, Request.Path);
            return Ok(response);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            Response.Cookies.Delete("jwt_token");
            _logger.LogInformation("[{Method}] {Url} - User logged out successfully", Request.Method, Request.Path);
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequestDto dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            await _service.ForgetPassword(dto);
            _logger.LogInformation("[{Method}] {Url} - Password reset email sent successfully", Request.Method, Request.Path);
            return Ok(new { message = "New Password is set and sent to your email." });
        }

    }
}
