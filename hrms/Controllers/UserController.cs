using AutoMapper;
using hrms.CustomException;
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
    //[Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _service;
        public UserController(IUserService service, IMapper mapper)
        {
            _service = service;
        }

        [HttpGet]
        //[Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> GetAll(int PageSize = 10,int PageNumber = 1) {
            if (PageNumber <= 0 || PageSize <= 0)
                throw new InvalidOperationCustomException($"{nameof(PageNumber)} and {nameof(PageSize)} size must be greater than 0.");
            PagedReponseDto<UserResponseDto> response = await _service.GetAllUser(PageSize,PageNumber);
            return Ok(response);
        }

        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetUser(int UserId)
        {
            var user = await _service.GetUserById(UserId);
            return Ok(user);
        }

        [HttpGet("search/employee")]
        [Authorize]
        public async Task<IActionResult> GetEmployeesByName(string? key)
        {
            if (key == null)
            {
                List<UserResponseDto> e =
                await _service.GetEmployees();
                return Ok(e);
            }
            string s = key;
            List<UserResponseDto> employees =
                await _service.GetEmployeesByName(s);
            return Ok(employees);
        }


        [HttpGet("search/hr")]
        [Authorize]
        public async Task<IActionResult> GetHrByKey(string? key)
        {
            if (key == null)
            {
                PagedReponseDto<UserResponseDto> res =
                await _service.GetAllHr(10, 1);
                List<UserResponseDto> e = res.Data;
                return Ok(e);
            }
            string s = key;
            List<UserResponseDto> hrs =
                await _service.GetHrByKey(s);
            return Ok(hrs);
        }

        [HttpGet("search/all")]
        [Authorize]
        public async Task<IActionResult> GetUserByKey(string? key)
        {
            if (key == null)
            {
                var res =
                await _service.GetAllUser(10,1);
                List<UserResponseDto> e = res.Data;
                return Ok(e);
            }
            string s = key;
            List<UserResponseDto> employees =
                await _service.GetUserByKey(s);
            return Ok(employees);
        }

        [HttpGet("{userId}/organization-chart")]
        public async Task<IActionResult> GetEmployeeOrganizationChart(int? userId)
        {
            if(userId == null)
            {
                throw new ArgumentNullException("userid not found !!");
            }
            List<UserResponseDto> organizationChart = await _service.GetUserChart((int)userId);
            return Ok(organizationChart);
        }
    }
}
