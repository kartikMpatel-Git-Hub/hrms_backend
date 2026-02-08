using hrms.Model;

namespace hrms.Service
{
    public interface IUserService
    {
        Task<List<User>> GetAllUser();
    }
}
