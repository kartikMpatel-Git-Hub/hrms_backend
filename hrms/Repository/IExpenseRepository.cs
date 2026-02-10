using hrms.Model;

namespace hrms.Repository
{
    public interface IExpenseRepository
    {
        Task<ExpenseCategory> CreateCategory(ExpenseCategory expenseCategory);
        Task<bool> ExistExpenseCategory(string category);
        Task<ExpenseCategory> GetCategoryById(int categoryId);
        Task<Expense> AddExpense(Expense expense);
        Task<ExpenseProof> AddProof(ExpenseProof proof);
        Task<List<ExpenseCategory>> GetAllExpenseCategory();
        Task<List<Expense>> GetAllTravelTravelerExpense(int travelId, int travelerId);
        Task<Expense> GetExpenseById(int expenseId);
        Task<Expense> UpdateExpenseStatus(Expense expense);
    }
}
