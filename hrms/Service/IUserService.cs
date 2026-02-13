using hrms.Dto.Response.Other;
using hrms.Dto.Response.Travel;
using hrms.Dto.Response.User;
using hrms.Model;

namespace hrms.Service
{
    public interface IUserService
    {
        Task<PagedReponseDto<UserResponseDto>> GetAllHr(int pageSize, int pageNumber);
        Task<PagedReponseDto<UserResponseDto>> GetAllUser(int PageSize,int PageNumber);
        Task<User> GetEmployee(int currentUserId);
        Task<List<UserResponseDto>> GetEmployees();
        Task<List<UserResponseDto>> GetEmployeesByName(string s);
        Task<List<UserResponseDto>> GetHrByKey(string s);
        Task<UserResponseDto> GetUserById(int UserId);
        Task<List<UserResponseDto>> GetUserByKey(string s);
        Task<User> GetUserEntityById(int UserId);
    }
}
