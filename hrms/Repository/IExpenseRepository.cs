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
    }
}
