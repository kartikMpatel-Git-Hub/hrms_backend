using hrms.CustomException;
using hrms.Dto.Request.Department;
using hrms.Dto.Response.Department;
using hrms.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace hrms.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    [EnableCors("MyAllowSpecificOrigins")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _service;
        public DepartmentController(IDepartmentService service)
        {
            _service = service;
        }

        [HttpGet("{departmentId}")]
        public async Task<IActionResult> GetDepartmentById(int? departmentId)
        {
            if (departmentId == null)
            {
                throw new NotFoundCustomException("Department Id Not Found !");
            }
            return Ok(await _service.GetDepartmentById((int)departmentId));
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            return Ok(await _service.GetDepartments());
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment(DepartmentCreateDto? dto)
        {
            if (dto == null)
                throw new ArgumentNullException("Department Create Body Not Found !");
            DepartmentResponseDto response = await _service.CreateDepartment((DepartmentCreateDto)dto);
            return Ok(response);    
        }
    }
}
