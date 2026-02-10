using AutoMapper;
using CloudinaryDotNet.Actions;
using hrms.Dto.Response.Expense;
using hrms.Dto.Response.Expense.Category;
using hrms.Dto.Response.Expense.Proof;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Travel;
using hrms.Dto.Response.Travel.Document;
using hrms.Dto.Response.Traveler;
using hrms.Dto.Response.User;
using hrms.Model;

namespace hrms.Utility
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<User, UserResponseDto>()
                .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => GetRoleName(src.Role)));
            CreateMap<Travel, TravelResponseDto>();
            CreateMap<Travel, TravelWithTravelerResponseDto>();
            CreateMap<Traveler, TravelerDto>();
            CreateMap<ExpenseCategory, ExpenseCategoryResponseDto>();
            CreateMap<Expense,ExpenseResponseDto>()
                .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => GetStatus(src.Status)));
            CreateMap<ExpenseProof, ExpenseProofResponseDto>();
            CreateMap<TravelDocument, TravelDocumentResponseDto>();

            CreateMap(typeof(PagedReponseOffSet<>),typeof(PagedReponseDto<>));
        }

        private object GetStatus(ExpenseStatus status)
        {
            switch (status)
            {
                case ExpenseStatus.APPROVED:
                    return "Approved";
                case ExpenseStatus.REJECTED:
                    return "Rejected";
                case ExpenseStatus.PENDING:
                    return "Pending";
                default:
                    return "Unknown";
            }
        }

        private object GetRoleName(UserRole role)
        {
            switch (role)
            {
                case UserRole.ADMIN:
                    return "Admin";
                case UserRole.HR:
                    return "Hr";
                case UserRole.MANAGER:
                    return "Manager";
                case UserRole.EMPLOYEE:
                    return "Employee";
                default:
                    return "Unknown";
            }
        }
    }
}
