using hrms.Model;

namespace hrms.Repository
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();
        Task<bool> ExistsByEmailAsync(String email);
        Task<User> GetByIdAsync(int? id);
        Task<User> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}
