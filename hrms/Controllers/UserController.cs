using AutoMapper;
using hrms.Dto.Response.User;
using hrms.Model;
using hrms.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace hrms.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _service;
        private readonly IMapper _mapper;
        public UserController(IUserService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() {
            List<User> users = _service.GetAllUser().Result;
            var responseDtos = _mapper.Map<List<UserResponseDto>>(users);
            return Ok(responseDtos);
        }
    }
}
