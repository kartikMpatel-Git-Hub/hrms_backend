using hrms.Dto.Response.Other;
using hrms.Model;

namespace hrms.Repository
{
    public interface IUserRepository
    {
        Task<PagedReponseOffSet<User>> GetAll(int PageSize,int PageNumber);
        Task<bool> ExistsByEmailAsync(String email);
        Task<User> GetByIdAsync(int? id);
        Task<User> GetById(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
        Task SaveChangesAsync();
        Task<User> GetManagerByIdAsync(int? managerId);
        Task<User> GetEmployeeById(int currentUserId);
        Task<User> GetHrById(int hrId);
        Task<List<User>> GetEmployeesByName(string s);
        Task<List<User>> GetUserByKey(string s);
        Task<List<User>> GetAllEmployee(int v1, int v2);
        Task<PagedReponseOffSet<User>> GetAllHrs(int pageSize, int pageNumber);
        Task<List<User>> GetHrByKey(string s);
        Task ToggleGameInterestStatus(int userId, int gameId);
    }
}
