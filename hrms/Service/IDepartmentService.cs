using hrms.Dto.Request.Department;
using hrms.Dto.Response.Department;
using Microsoft.AspNetCore.Mvc;

namespace hrms.Service
{
    public interface IDepartmentService
    {
        Task<DepartmentResponseDto> CreateDepartment(DepartmentCreateDto dto);
        Task<DepartmentResponseDto> GetDepartmentById(int departmentId);
        Task<List<DepartmentResponseDto>> GetDepartments();
    }
}
