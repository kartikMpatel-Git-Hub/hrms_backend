using hrms.Model;
using hrms.Repository;

namespace hrms.Service.impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<User>> GetAllUser()
        {
            return await _repository.GetAll();
        }
    }
}
