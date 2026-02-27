using AutoMapper;
using hrms.CustomException;
using hrms.Dto.Request.User;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.User;
using hrms.Model;
using hrms.Repository;
using hrms.Utility;
using Microsoft.Extensions.Caching.Memory;

namespace hrms.Service.impl
{
    public class UserService(
        IUserRepository _repository, 
        IMapper _mapper, 
        ICloudinaryService _cloudinaryService,
        ILogger<UserService> _logger,
        IMemoryCache _cache
        ) : IUserService
    {
        public async Task<UserResponseDto> GetUserById(int UserId)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForUserInfo(UserId));
            var key = $"UserInfo:UserId:{UserId}:version:{version}";
            if (_cache.TryGetValue(key, out UserResponseDto cachedUser))
            {
                _logger.LogDebug("Cache hit for user info with id {UserId} (version {Version})", UserId, version);
                return cachedUser;
            }
            var user = await GetUserEntityById(UserId);
            var response = _mapper.Map<UserResponseDto>(user);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved user info with id {UserId} from repository and cached with version {Version}", UserId, version);
            return response;
        }

        public async Task<PagedReponseDto<UserResponseDto>> GetAllUser(int PageSize, int PageNumber)
        {
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.UserDetails));
            var key = $"Users:PageSize:{PageSize}:PageNumber:{PageNumber}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<UserResponseDto> cachedUsers))
            {
                _logger.LogDebug("Cache hit for all users (version {Version})", version);
                return cachedUsers;
            }
            PagedReponseOffSet<User> PageUsers = await _repository.GetAll(PageSize, PageNumber);
            var response = _mapper.Map<PagedReponseDto<UserResponseDto>>(PageUsers);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved all users with version {Version} and cached", version);
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
            var key = _cache.Get<int>(CacheVersionKey.For(CacheDomains.UserDetails));
            var cacheKey = $"Employees:version:{key}";
            if (_cache.TryGetValue(cacheKey, out List<UserResponseDto> cachedEmployees))
            {
                _logger.LogDebug("Cache hit for employees (version {Version})", key);
                return cachedEmployees;
            }
            List<User> employees = await _repository.GetAllEmployee(10, 1);
            var response = _mapper.Map<List<UserResponseDto>>(employees);
            _cache.Set(cacheKey, response, TimeSpan.FromMinutes(60));
            return response;
        }

        public async Task<List<UserResponseDto>> GetUserByKey(string s)
        {
            List<User> employees = await _repository.GetUserByKey(s);
            return _mapper.Map<List<UserResponseDto>>(employees);
        }

        public async Task<PagedReponseDto<UserResponseDto>> GetAllHr(int pageSize, int pageNumber)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForHrInfo());
            var key = $"Hrs:PageSize:{pageSize}:PageNumber:{pageNumber}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<UserResponseDto> cachedHrs))
            {
                _logger.LogDebug("Cache hit for all hrs (version {Version})", version);
                return cachedHrs;
            }
            PagedReponseOffSet<User> PageHrs = await _repository.GetAllHrs(pageSize, pageNumber);
            var response = _mapper.Map<PagedReponseDto<UserResponseDto>>(PageHrs);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved all hrs with version {Version} and cached", version);
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
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.UserDetails));
            var key = $"AllUserForHr:PageSize:{pageSize}:PageNumber:{pageNumber}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<UserResponseDto> cachedUsers))
            {
                _logger.LogDebug("Cache hit for all users for hr (version {Version})", version);
                return cachedUsers;
            }
            PagedReponseOffSet<User> PageUsers = await _repository.GetAllUserForHr(pageSize, pageNumber);
            var response = _mapper.Map<PagedReponseDto<UserResponseDto>>(PageUsers);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved all users for hr with version {Version} and cached", version);
            return response;
        }

        public async Task<PagedReponseDto<UserResponseDto>> GetAllManagers(int pageSize, int pageNumber)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForManagerInfo());
            var key = $"Managers:PageSize:{pageSize}:PageNumber:{pageNumber}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<UserResponseDto> cachedManagers))
            {
                _logger.LogDebug("Cache hit for all managers (version {Version})", version);
                return cachedManagers;
            }
            PagedReponseOffSet<User> PageUsers = await _repository.GetAllManagers(pageSize, pageNumber);
            var response = _mapper.Map<PagedReponseDto<UserResponseDto>>(PageUsers);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved all managers with version {Version} and cached", version);
            return response;
        }

        public async Task<PagedReponseDto<UserResponseDto>> GetEmployeeUnderManager(int userId, int pageSize, int pageNumber)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForManagerTeam());
            var key = $"EmployeeUnderManager:UserId:{userId}:PageSize:{pageSize}:PageNumber:{pageNumber}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<UserResponseDto> cachedEmployees))
            {
                _logger.LogDebug("Cache hit for employees under manager (version {Version})", version);
                return cachedEmployees;
            }
            PagedReponseOffSet<User> PageUsers = await _repository.GetEmployeeUnderManager(userId, pageSize, pageNumber);
            var response = _mapper.Map<PagedReponseDto<UserResponseDto>>(PageUsers);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved employees under manager with version {Version} and cached", version);
            return response;
        }

        public async Task<UserProfileResponseDto> GetUserProfile(int userId)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForUserProfile(userId));
            var key = $"UserProfile:UserId:{userId}:version:{version}";
            if (_cache.TryGetValue(key, out UserProfileResponseDto cachedProfile))
            {
                _logger.LogDebug("Cache hit for user profile with id {UserId} (version {Version})", userId, version);
                return cachedProfile;
            }
            User user = await _repository.GetUserProfile(userId);
            var response = _mapper.Map<UserProfileResponseDto>(user);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved user profile with id {UserId} and cached", userId);
            return response;
        }

        public async Task UpdateUser(int userId, UserUpdateRequestDto userUpdateRequestDto)
        {
            User user = await UpdateUserData(userId, userUpdateRequestDto);
            await _repository.UpdateAsync(user);
            IncreaseCacheVersion(CacheVersionKey.ForUserInfo(userId));
            IncreaseCacheVersion(CacheVersionKey.ForUserDetails(userId));
            IncreaseCacheVersion(CacheVersionKey.ForUserProfile(userId));
            IncreaseCacheVersion(CacheVersionKey.For(CacheDomains.UserDetails));
            IncreaseCacheVersion(CacheVersionKey.ForHrInfo());
            IncreaseCacheVersion(CacheVersionKey.ForManagerInfo());
            IncreaseCacheVersion(CacheVersionKey.ForEmployeeInfo());
            IncreaseCacheVersion(CacheVersionKey.ForManagerTeam());

            _logger.LogInformation("Updated user with id {UserId} and increased cache version", userId);
        }

        private void IncreaseCacheVersion(string v)
        {
            var current = _cache.Get<int>(v);
            _cache.Set(v, current + 1);
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
