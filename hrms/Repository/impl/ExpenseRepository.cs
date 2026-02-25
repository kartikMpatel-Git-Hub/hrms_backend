using hrms.CustomException;
using hrms.Data;
using hrms.Dto.Response.Other;
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

        public async Task<List<ExpenseCategory>> GetAllExpenseCategory()
        {
            List<ExpenseCategory> categories
                = await _db.ExpenseCategories.ToListAsync();
            if (categories == null)
                throw new NotFoundCustomException("Expense Category Not Found !");
            return categories;
        }

        public async Task<PagedReponseOffSet<Expense>> GetAllExpenses(int pageNumber, int pageSize, int currentUserId)
        {
            int totalRecords = await _db.Expenses
                            .Where(e => e.Travel.CreatedBy == currentUserId)
                            .CountAsync();
            List<Expense> expenses
                = _db.Expenses
                .Include(e => e.Category)
                .Include(e => e.Proofs)
                .Where(e => e.Travel.CreatedBy == currentUserId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(e => e.ExpenseDate)
                .ToList();
            PagedReponseOffSet<Expense> response = new PagedReponseOffSet<Expense>(expenses, pageNumber, pageSize,totalRecords);
            return response;
        }

        public async Task<List<Expense>> GetAllTravelTravelerExpense(int travelId, int travelerId)
        {
            List<Expense> expenses
                = await _db.Expenses
                .Where((e) => e.TravelId == travelId && e.TravelerId == travelerId)
                .Include(e => e.Category)
                .Include(e => e.Proofs)
                .ToListAsync();
            return expenses;
        }

        public async Task<ExpenseCategory> GetCategoryById(int categoryId)
        {
            ExpenseCategory category = await _db.ExpenseCategories
                .FirstOrDefaultAsync((c) => c.Id == categoryId);
            if (category == null)
                throw new NotFoundCustomException($"Category With Id : {categoryId} Not Found !");
            return category;
        }

        public async Task<Expense> GetExpenseById(int expenseId)
        {
            Expense expense = await _db.Expenses
                .FirstOrDefaultAsync((e) => e.Id == expenseId);
            if (expense == null)
                throw new NotFoundCustomException($"Expense With Id : {expenseId} Not Found !");
            return expense;
        }

        public async Task<Expense> UpdateExpenseStatus(Expense expense)
        {
            var UpdatedEntity = _db.Update(expense);
            await _db.SaveChangesAsync();
            return UpdatedEntity.Entity;
        }
    }
}
