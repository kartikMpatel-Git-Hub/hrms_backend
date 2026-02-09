using hrms.Dto.Request.Category;
using hrms.Dto.Request.Expense;
using hrms.Dto.Request.Travel;
using hrms.Dto.Response.Expense;
using hrms.Dto.Response.Expense.Category;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Travel;
using hrms.Dto.Response.User;
using hrms.Model;

namespace hrms.Service
{
    public interface ITravelService
    {
        Task<TravelResponseDto> CreateTravelAsync(TravelCreateDto Dto,int CurrentUser);
        Task<TravelResponseDto> GetTravelByIdAsync(int TravelId);
        Task<TravelWithTravelerResponseDto> GetTravelersByTravelId(int TravelId);
        Task<PagedReponseDto<TravelResponseDto>> GetHrCreatedTravels(int HrId, int PageSize, int PageNumber);
        Task RemoveTravel(int TravelId);
        Task<TravelResponseDto> UpdateTravelById(int TravelId,TravelUpdateDto dto);
        Task AddTraveler(int currentUserId, TravelerAddDto dto);
        Task<ExpenseCategoryResponseDto> CreateExpenseCategory(ExpenseCategoryCreateDto dto);
        Task<ExpenseResponseDto> AddExpense(int travelId, int currentUserId, ExpenseCreateDto dto, List<IFormFile> files);
    }
}
