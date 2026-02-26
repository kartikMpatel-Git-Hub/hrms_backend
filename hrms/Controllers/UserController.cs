using System.Security.Claims;
using AutoMapper;
using hrms.CustomException;
using hrms.Dto.Request.User;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Travel;
using hrms.Dto.Response.User;
using hrms.Model;
using hrms.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace hrms.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _service;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService service, IMapper mapper, ILogger<UserController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        //[Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> GetAll(int PageSize = 10, int PageNumber = 1)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (PageNumber <= 0 || PageSize <= 0)
                throw new InvalidOperationCustomException($"{nameof(PageNumber)} and {nameof(PageSize)} size must be greater than 0.");
            PagedReponseDto<UserResponseDto> response = await _service.GetAllUser(PageSize, PageNumber);
            _logger.LogInformation("[{Method}] {Url} - Fetched all users successfully", Request.Method, Request.Path);
            return Ok(response);
        }

        [HttpGet("user-hr")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetAllHr(int PageSize = 10, int PageNumber = 1)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (PageNumber <= 0 || PageSize <= 0)
                throw new InvalidOperationCustomException($"{nameof(PageNumber)} and {nameof(PageSize)} size must be greater than 0.");
            PagedReponseDto<UserResponseDto> response = await _service.GetAllUserForHr(PageSize, PageNumber);
            _logger.LogInformation("[{Method}] {Url} - Fetched HR users successfully", Request.Method, Request.Path);
            return Ok(response);
        }

        [HttpGet("managers")]
        public async Task<IActionResult> GetManager(int PageSize = 10, int PageNumber = 1)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (PageNumber <= 0 || PageSize <= 0)
                throw new InvalidOperationCustomException($"{nameof(PageNumber)} and {nameof(PageSize)} size must be greater than 0.");
            PagedReponseDto<UserResponseDto> response = await _service.GetAllManagers(PageSize, PageNumber);
            _logger.LogInformation("[{Method}] {Url} - Fetched managers successfully", Request.Method, Request.Path);
            return Ok(response);
        }

        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetUser(int? UserId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (UserId == null)
            {
                return BadRequest("UserId is required.");
            }
            UserProfileResponseDto user = await _service.GetUserProfile((int)UserId);
            _logger.LogInformation("[{Method}] {Url} - Fetched profile for user {UserId} successfully", Request.Method, Request.Path, UserId);
            return Ok(user);
        }

        [HttpPut("{UserId}")]
        public async Task<IActionResult> UpdateUser(int? UserId, UserUpdateRequestDto userUpdateRequestDto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (UserId == null)
            {
                return BadRequest("UserId is required.");
            }
            await _service.UpdateUser((int)UserId, userUpdateRequestDto);
            _logger.LogInformation("[{Method}] {Url} - User {UserId} updated successfully", Request.Method, Request.Path, UserId);
            return Ok("User Updated Successfully");
        }

        [HttpGet("search/employee")]
        [Authorize]
        public async Task<IActionResult> GetEmployeesByName(string? key)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (key == null)
            {
                List<UserResponseDto> e =
                await _service.GetEmployees();
                _logger.LogInformation("[{Method}] {Url} - Fetched all employees successfully", Request.Method, Request.Path);
                return Ok(e);
            }
            string s = key;
            List<UserResponseDto> employees =
                await _service.GetEmployeesByName(s);
            _logger.LogInformation("[{Method}] {Url} - Fetched employees matching '{Key}' successfully", Request.Method, Request.Path, key);
            return Ok(employees);
        }


        [HttpGet("search/hr")]
        [Authorize]
        public async Task<IActionResult> GetHrByKey(string? key)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (key == null)
            {
                PagedReponseDto<UserResponseDto> res =
                await _service.GetAllHr(10, 1);
                List<UserResponseDto> e = res.Data;
                _logger.LogInformation("[{Method}] {Url} - Fetched all HR users successfully", Request.Method, Request.Path);
                return Ok(e);
            }
            string s = key;
            List<UserResponseDto> hrs =
                await _service.GetHrByKey(s);
            _logger.LogInformation("[{Method}] {Url} - Fetched HR users matching '{Key}' successfully", Request.Method, Request.Path, key);
            return Ok(hrs);
        }

        [HttpGet("search/all")]
        [Authorize]
        public async Task<IActionResult> GetUserByKey(string? key)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (key == null)
            {
                var res =
                await _service.GetAllUser(10, 1);
                List<UserResponseDto> e = res.Data;
                _logger.LogInformation("[{Method}] {Url} - Fetched all users successfully", Request.Method, Request.Path);
                return Ok(e);
            }
            string s = key;
            List<UserResponseDto> employees =
                await _service.GetUserByKey(s);
            _logger.LogInformation("[{Method}] {Url} - Fetched users matching '{Key}' successfully", Request.Method, Request.Path, key);
            return Ok(employees);
        }

        [HttpGet("{userId}/organization-chart")]
        public async Task<IActionResult> GetEmployeeOrganizationChart(int? userId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (userId == null)
            {
                throw new ArgumentNullException("userid not found !!");
            }
            List<UserResponseDto> organizationChart = await _service.GetUserChart((int)userId);
            _logger.LogInformation("[{Method}] {Url} - Fetched organization chart for user {UserId} successfully", Request.Method, Request.Path, userId);
            return Ok(organizationChart);
        }

        [HttpGet("my-team")]
        [Authorize(Roles = "MANAGER")]
        public async Task<IActionResult> GetEmployeeWithManager(int PageSize = 10, int PageNumber = 1)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            var user = User;
            if (user == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int userId = Int32.Parse(user.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<UserResponseDto> response = await _service.GetEmployeeUnderManager(userId, PageSize, PageNumber);
            _logger.LogInformation("[{Method}] {Url} - Fetched team members for manager {UserId} successfully", Request.Method, Request.Path, userId);
            return Ok(response);
        }
    }
}
