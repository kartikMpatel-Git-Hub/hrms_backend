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
        private readonly ILogger<DepartmentController> _logger;
        public DepartmentController(IDepartmentService service, ILogger<DepartmentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("{departmentId}")]
        public async Task<IActionResult> GetDepartmentById(int? departmentId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (departmentId == null)
            {
                throw new NotFoundCustomException("Department Id Not Found !");
            }
            var result = await _service.GetDepartmentById((int)departmentId);
            _logger.LogInformation("[{Method}] {Url} - Fetched department with ID {DepartmentId} successfully", Request.Method, Request.Path, departmentId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            var result = await _service.GetDepartments();
            _logger.LogInformation("[{Method}] {Url} - Fetched all departments successfully", Request.Method, Request.Path);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment(DepartmentCreateDto? dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (dto == null)
                throw new ArgumentNullException("Department Create Body Not Found !");
            DepartmentResponseDto response = await _service.CreateDepartment((DepartmentCreateDto)dto);
            _logger.LogInformation("[{Method}] {Url} - Department '{Name}' created successfully", Request.Method, Request.Path, dto.DepartmentName);
            return Ok(response);    
        }
    }
}
