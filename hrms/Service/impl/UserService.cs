using AutoMapper;
using hrms.CustomException;
using hrms.Dto.Request.User;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.User;
using hrms.Model;
using hrms.Repository;

namespace hrms.Service.impl
{
    public class UserService(IUserRepository _repository, IMapper _mapper, ICloudinaryService _cloudinaryService) : IUserService
    {
        public async Task<UserResponseDto> GetUserById(int UserId)
        {
            var user = GetUserEntityById(UserId);
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<PagedReponseDto<UserResponseDto>> GetAllUser(int PageSize, int PageNumber)
        {
            PagedReponseOffSet<User> PageUsers = await _repository.GetAll(PageSize, PageNumber);
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
            List<User> employees = await _repository.GetAllEmployee(10, 1);

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
            chain.Reverse();
            return _mapper.Map<List<UserResponseDto>>(chain);
        }

        public Task ToggleGameInterestStatus(int userId, int gameId)
        {
            return _repository.ToggleGameInterestStatus(userId, gameId);
        }

        public async Task<PagedReponseDto<UserResponseDto>> GetAllUserForHr(int pageSize, int pageNumber)
        {
            PagedReponseOffSet<User> PageUsers = await _repository.GetAllUserForHr(pageSize, pageNumber);
            return _mapper.Map<PagedReponseDto<UserResponseDto>>(PageUsers);
        }

        public async Task<PagedReponseDto<UserResponseDto>> GetAllManagers(int pageSize, int pageNumber)
        {
            PagedReponseOffSet<User> PageUsers = await _repository.GetAllManagers(pageSize, pageNumber);
            return _mapper.Map<PagedReponseDto<UserResponseDto>>(PageUsers);
        }

        public async Task<PagedReponseDto<UserResponseDto>> GetEmployeeUnderManager(int userId, int pageSize, int pageNumber)
        {
            PagedReponseOffSet<User> PageUsers = await _repository.GetEmployeeUnderManager(userId, pageSize, pageNumber);
            return _mapper.Map<PagedReponseDto<UserResponseDto>>(PageUsers);
        }

        public async Task<UserProfileResponseDto> GetUserProfile(int userId)
        {
            User user = await _repository.GetUserProfile(userId);
            return _mapper.Map<UserProfileResponseDto>(user);
        }

        public async Task UpdateUser(int userId, UserUpdateRequestDto userUpdateRequestDto)
        {
            User user = await UpdateUserData(userId, userUpdateRequestDto);
            await _repository.UpdateAsync(user);
        }

        private async Task<User> UpdateUserData(int userId, UserUpdateRequestDto dto)
        {
            User user = await _repository.GetById(userId);
            if (dto.FullName != null) user.FullName = dto.FullName;
            if (dto.Email != null) user.Email = dto.Email;
            if (dto.DateOfBirth != null) user.DateOfBirth = (DateTime)dto.DateOfBirth;
            if (dto.DateOfJoin != null) user.DateOfJoin = (DateTime)dto.DateOfJoin;
            if (dto.ReportTo != null) user.ReportTo = dto.ReportTo;
            if (dto.DepartmentId != null) user.DepartmentId = dto.DepartmentId;
            if (dto.Designation != null) user.Designation = dto.Designation;
            if (dto.Image != null)
            {
                user.Image = await _cloudinaryService.UploadAsync(dto.Image);
            }
            return user;
        }
    }
}
