using AutoMapper;
using hrms.CustomException;
using hrms.Dto.Request.Department;
using hrms.Dto.Response.Department;
using hrms.Model;
using hrms.Repository;
using hrms.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace hrms.Service.impl
{
    public class DepartmentService(
        IDepartmentRepository _repository,
        IMapper _mapper,
        ILogger<DepartmentService> _logger,
        IMemoryCache _cache
    ) : IDepartmentService
    {
        public async Task<DepartmentResponseDto> CreateDepartment(DepartmentCreateDto dto)
        {

            if (await _repository.DepartmentExists(dto.DepartmentName))
            {
                throw new ExistsCustomException("Department Already Exist In System !");
            }
            Department department = new Department()
            {
                DepartmentName = dto.DepartmentName,
            };
            Department d = await _repository.CreateDepartment(department);
            var key = CacheVersionKey.For(CacheDomains.DepartmentDetails);
            var current = _cache.Get<int>(key);
            _cache.Set(key, current + 1);
            return _mapper.Map<DepartmentResponseDto>(d);
        }

        public async Task<DepartmentResponseDto> GetDepartmentById(int departmentId)
        {
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.DepartmentDetails));
            var key = $"DepartmentById:{departmentId}:version:{version}";
            if (_cache.TryGetValue(key, out DepartmentResponseDto cachedDepartment))
            {
                _logger.LogDebug("Cache hit for department with id {DepartmentId} (version {Version})", departmentId, version);
                return cachedDepartment;
            }
            Department department = await _repository.GetDepartmentById(departmentId);
            var result = _mapper.Map<DepartmentResponseDto>(department);
            _cache.Set(key, result, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved department with id {DepartmentId} from repository and cached with version {Version}", departmentId, version);
            return result;
        }

        public async Task<List<DepartmentResponseDto>> GetDepartments()
        {
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.DepartmentDetails));
            var key = $"Departments:version:{version}";
            if (_cache.TryGetValue(key, out List<DepartmentResponseDto> cachedDepartments))
            {
                _logger.LogDebug("Cache hit for departments (version {Version})", version);
                return cachedDepartments;
            }
            List<Department> departments = await _repository.GetDepartments();
            List<DepartmentResponseDto> departmentResponseDtos = _mapper.Map<List<DepartmentResponseDto>>(departments);
            _cache.Set(key, departmentResponseDtos, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved {Count} departments from repository and cached with version {Version}", departments.Count, version);
            return departmentResponseDtos;
        }
    }
}
