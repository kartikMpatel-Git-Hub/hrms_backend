using hrms.Dto.Response.Other;
using hrms.Dto.Response.Travel;
using hrms.Dto.Response.User;
using hrms.Model;

namespace hrms.Service
{
    public interface IUserService
    {
        Task<PagedReponseDto<UserResponseDto>> GetAllHr(int pageSize, int pageNumber);
        Task<PagedReponseDto<UserResponseDto>> GetAllManagers(int pageSize, int pageNumber);
        Task<PagedReponseDto<UserResponseDto>> GetAllUser(int PageSize,int PageNumber);
        Task<PagedReponseDto<UserResponseDto>> GetAllUserForHr(int pageSize, int pageNumber);
        Task<User> GetEmployee(int currentUserId);
        Task<List<UserResponseDto>> GetEmployees();
        Task<List<UserResponseDto>> GetEmployeesByName(string s);
        Task<PagedReponseDto<UserResponseDto>> GetEmployeeUnderManager(int userId, int pageSize, int pageNumber);
        Task<List<UserResponseDto>> GetHrByKey(string s);
        Task<UserResponseDto> GetUserById(int UserId);
        Task<List<UserResponseDto>> GetUserByKey(string s);
        Task<List<UserResponseDto>> GetUserChart(int userId);
        Task<User> GetUserEntityById(int UserId);
        Task<UserProfileResponseDto> GetUserProfile(int userId);
        Task ToggleGameInterestStatus(int userId, int gameId);
    }
}
