using AutoMapper;
using hrms.CustomException;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.User;
using hrms.Model;
using hrms.Repository;
using Microsoft.EntityFrameworkCore;

namespace hrms.Service.impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<UserResponseDto> GetUserById(int UserId)
        {
            var user = GetUserEntityById(UserId);
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<PagedReponseDto<UserResponseDto>> GetAllUser(int PageSize,int PageNumber)
        {
            PagedReponseOffSet<User> PageUsers = await _repository.GetAll(PageSize,PageNumber);
            var response = _mapper.Map<PagedReponseDto<UserResponseDto>>(PageUsers);
            return response;
        }
        public async Task<User> GetUserEntityById(int userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundCustomException($"User Not Found with Id : {userId}");
            return user;
        }

        public async Task<User> GetEmployee(int currentUserId)
        {
            var response = await _repository.GetEmployeeById(currentUserId);
            return response;
        }

        public async Task<List<UserResponseDto>> GetEmployeesByName(string s)
        {
            List<User> employees = await _repository.GetEmployeesByName(s);
            return _mapper.Map<List<UserResponseDto>>(employees);
        }

        public async Task<List<UserResponseDto>> GetEmployees()
        {
            List<User> employees = await _repository.GetAllEmployee(10,1);
            
            return _mapper.Map<List<UserResponseDto>>(employees);
        }

        public async Task<List<UserResponseDto>> GetUserByKey(string s)
        {
            List<User> employees = await _repository.GetUserByKey(s);
            return _mapper.Map<List<UserResponseDto>>(employees);
        }

        public async Task<PagedReponseDto<UserResponseDto>> GetAllHr(int pageSize, int pageNumber)
        {
            PagedReponseOffSet<User> PageHrs = await _repository.GetAllHrs(pageSize, pageNumber);
            var response = _mapper.Map<PagedReponseDto<UserResponseDto>>(PageHrs);
            return response;
        }

        public async Task<List<UserResponseDto>> GetHrByKey(string s)
        {
            List<User> hrs = await _repository.GetHrByKey(s);
            return _mapper.Map<List<UserResponseDto>>(hrs);
        }

        public async Task<List<UserResponseDto>> GetUserChart(int userId)
        {
            var chain = new List<User>();
            var currentUser = await _repository.GetById(userId);
            chain.Add(currentUser);
            while (currentUser != null && currentUser.ReportTo.HasValue)
            {
                currentUser = await _repository.GetById((int)currentUser.ReportTo);
                if (currentUser != null)
                {
                    chain.Add(currentUser);
                }
            }
            return _mapper.Map<List<UserResponseDto>>(chain);
        }

        public Task ToggleGameInterestStatus(int userId, int gameId)
        {
            return _repository.ToggleGameInterestStatus(userId, gameId);
        }
    }
}
