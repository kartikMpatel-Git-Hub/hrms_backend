using hrms.Dto.Request.Category;
using hrms.Dto.Request.Expense;
using hrms.Dto.Response.Expense;
using hrms.Dto.Response.Expense.Category;

namespace hrms.Service
{
    public interface IExpenseService
    {
        Task<ExpenseCategoryResponseDto> CreateExpenseCategory(ExpenseCategoryCreateDto dto);
        Task<ExpenseResponseDto> AddExpense(int travelId, int currentUserId, ExpenseCreateDto dto, List<IFormFile> files);
    }
}
