using AutoMapper;
using hrms.CustomException;
using hrms.Dto.Request.Department;
using hrms.Dto.Response.Department;
using hrms.Model;
using hrms.Repository;
using Microsoft.AspNetCore.Mvc;

namespace hrms.Service.impl
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;
        public DepartmentService(IDepartmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<DepartmentResponseDto> CreateDepartment(DepartmentCreateDto dto)
        {
            if(await _repository.DepartmentExists(dto.DepartmentName))
            {
                throw new ExistsCustomException("Department Already Exist In System !");
            }
            Department department = new Department()
            {
                DepartmentName = dto.DepartmentName,
            };
            return _mapper.Map<DepartmentResponseDto>(await _repository.CreateDepartment(department));
        }

        public async Task<DepartmentResponseDto> GetDepartmentById(int departmentId)
        {
            return _mapper.Map<DepartmentResponseDto>(await _repository.GetDepartmentById(departmentId));
        }

        public async Task<List<DepartmentResponseDto>> GetDepartments()
        {
            return _mapper.Map<List<DepartmentResponseDto>>
                (await _repository.GetDepartments());
        }
    }
}
