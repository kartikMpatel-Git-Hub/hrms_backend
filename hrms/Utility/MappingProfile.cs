using AutoMapper;
using hrms.Model;
using hrms.Dto.Response.User;

namespace hrms.Utility
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<User, UserResponseDto>();
        }
    }
}
