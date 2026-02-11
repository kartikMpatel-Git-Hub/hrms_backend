using hrms.Model;

namespace hrms.Repository
{
    public interface IDepartmentRepository
    {
        Task<Department> CreateDepartment(Department department);
        Department UpdateDepartment(Department department);
        Task<Department> DeleteDepartment(int deptId);
        Task<Department> GetDepartmentById(int deptId);
        Task<List<Department>> GetDepartments();
        Task<bool> DepartmentExists(string departmentName);
    }
}
