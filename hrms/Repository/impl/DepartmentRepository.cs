using hrms.CustomException;
using hrms.Data;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Repository.impl
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<DepartmentRepository> _logger;
        public DepartmentRepository(ApplicationDbContext db, ILogger<DepartmentRepository> logger) {
            _db = db;
            _logger = logger;
        }
        public async Task<Department> CreateDepartment(Department department)
        {
            var AddedEntity = await _db.Departments.AddAsync(department);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created Department with Id {Id}", AddedEntity.Entity.Id);
            return AddedEntity.Entity;
        }

        public async Task<Department> DeleteDepartment(int deptId)
        {
            Department department = await GetDepartmentById(deptId);
            department.is_deleted = true;
            await _db.SaveChangesAsync();
            _logger.LogInformation("Soft-deleted Department with Id {Id}", deptId);
            return department;
        }

        public async Task<bool> DepartmentExists(string departmentName)
        {
            return await _db.Departments
                .AnyAsync((d) => d.DepartmentName.ToLower().Trim() == departmentName.ToLower().Trim());
        }

        public async Task<Department> GetDepartmentById(int deptId)
        {
            Department department = await _db.Departments
                .Where((d)=>d.Id == deptId && !d.is_deleted)
                .FirstOrDefaultAsync();
            if (department == null)
                throw new NotFoundCustomException($"Department With Id : {deptId} Not Found !");
            _logger.LogInformation("Fetched Department with Id {Id}", deptId);
            return department;
        }

        public async Task<List<Department>> GetDepartments()
        {
            List<Department> departments = await _db.Departments
                .Where(d => d.is_deleted == false).ToListAsync();
            _logger.LogInformation("Fetched {Count} departments", departments.Count);
            return departments;
        }

        public Department UpdateDepartment(Department department)
        {
            Department UpdatedDepartment = _db.Departments.Update(department).Entity;
            return UpdatedDepartment;
        }
    }
}
