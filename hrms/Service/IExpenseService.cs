using hrms.dto.request.Expense;
using hrms.Dto.Request.Category;
using hrms.Dto.Request.Expense;
using hrms.Dto.Response.Expense;
using hrms.Dto.Response.Expense.Category;
using hrms.Dto.Response.Other;

namespace hrms.Service
{
    public interface IExpenseService
    {
        Task<ExpenseCategoryResponseDto> CreateExpenseCategory(ExpenseCategoryCreateDto dto);
        Task<ExpenseResponseDto> AddExpense(int travelId, int currentUserId, ExpenseCreateDto dto, List<IFormFile> files);
        Task<List<ExpenseCategoryResponseDto>> GetExpenseCategory();
        Task<List<ExpenseResponseDto>> GetTravelTravelerExpense(int travelId, int travelerId);
        Task<ExpenseResponseDto> ChangeExpenseStatus(int travelId, int travelerId, int expenseId,ExpenseStatusChangeDto dto);
        Task<PagedReponseDto<ExpenseResponseDto>> GetAllExpenses(int pageNumber, int pageSize, int currentUserId);
        Task<ExpenseResponseDto> UpdateExpense(int travelId, int expenseId, int currentUserId, ExpenseUpdateDto dto);
    }
}
