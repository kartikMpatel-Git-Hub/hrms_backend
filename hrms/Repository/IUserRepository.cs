using hrms.Dto.Response.Other;
using hrms.Model;

namespace hrms.Repository
{
    public interface IUserRepository
    {
        Task<PagedReponseOffSet<User>> GetAll(int PageSize,int PageNumber);
        Task<bool> ExistsByEmailAsync(String email);
        Task<User> GetByIdAsync(int? id);
        Task<User> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
        Task SaveChangesAsync();
        Task<User> GetManagerByIdAsync(int? managerId);
        Task<User> GetEmployeeById(int currentUserId);
        Task<List<User>> GetEmployeesByName(string s);
        Task<List<User>> GetAllEmployee(int v1, int v2);
    }
}
