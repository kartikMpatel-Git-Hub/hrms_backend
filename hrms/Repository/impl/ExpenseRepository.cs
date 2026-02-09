using hrms.CustomException;
using hrms.Data;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Repository.impl
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ApplicationDbContext _db;

        public ExpenseRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Expense> AddExpense(Expense expense)
        {
            var AddedEntity = await _db.AddAsync(expense);
            await _db.SaveChangesAsync();
            var AddedExpense = AddedEntity.Entity;
            return AddedExpense;
        }

        public async Task<ExpenseProof> AddProof(ExpenseProof proof)
        {
            var AddedEntity = await _db.ExpenseProofs.AddAsync(proof);
            await _db.SaveChangesAsync();
            var Addedprooft = AddedEntity.Entity;
            return Addedprooft;
        }

        public async Task<ExpenseCategory> CreateCategory(ExpenseCategory expenseCategory)
        {
            var AddedEntity = await _db.AddAsync(expenseCategory);
            await _db.SaveChangesAsync();
            var AddedCategory = AddedEntity.Entity;
            return AddedCategory;
        }

        public async Task<bool> ExistExpenseCategory(string category)
        {
            ExpenseCategory expenseCategory = await _db.ExpenseCategories.FirstOrDefaultAsync((c) => c.Category.ToLower().Trim() == category.ToLower().Trim());
            if (expenseCategory == null)
                return false;
            return true;
        }

        public async Task<ExpenseCategory> GetCategoryById(int categoryId)
        {
            ExpenseCategory category = await _db.ExpenseCategories
                .FirstOrDefaultAsync((c) => c.Id == categoryId);
            if (category == null)
                throw new NotFoundCustomException($"Category With Id : {categoryId} Not Found !");
            return category;
        }

    }
}
