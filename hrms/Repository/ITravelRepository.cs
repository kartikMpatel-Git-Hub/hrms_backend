using hrms.Dto.Response.Other;
using hrms.Model;

namespace hrms.Repository
{
    public interface ITravelRepository
    {
        Task<Travel> CreateTravel(Travel travel);

        Task<bool> UserExistsInTravel(int TravelId, int UserId);
        Task AddTraveler(Traveler traveler);
        Task<Travel> UpdateTravel(Travel travel);
        Task<Travel> GetTravelById(int TravelId);
        Task DeleteTravel(int TravelId);
        Task<PagedReponseOffSet<Travel>> GetTravelCreatedByHr(int HrId,int PageSize,int PageNumber);
        Task<Travel> GetTravelerByTravel(int travelId);
        Task<ExpenseCategory> CreateCategory(ExpenseCategory expenseCategory);
        Task<bool> ExistExpenseCategory(string category);
        Task<ExpenseCategory> GetCategoryById(int categoryId);
        Task<Expense> AddExpense(Expense expense);
    }
}
