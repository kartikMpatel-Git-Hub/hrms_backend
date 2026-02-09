using hrms.CustomException;
using hrms.Data;
using hrms.Dto.Response.Other;
using hrms.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace hrms.Repository.impl
{
    public class TravelRepository : ITravelRepository
    {
        private readonly ApplicationDbContext _db;

        public TravelRepository(ApplicationDbContext db)
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

        public async Task AddTraveler(Traveler traveler)
        {
            await _db.Travelers.AddAsync(traveler);
            Notification notification = new Notification()
            {
                NotifiedTo = traveler.TravelerId,
                Notified = traveler.Travelerr,
                Title = "Travel Booking",
                Description = $"Your Trip is Booked For {traveler.Travel.Location}",
                IsViewed = false,
                NotificationDate = DateTime.Now,
            };

            await _db.Notifications.AddAsync(notification);
            await _db.SaveChangesAsync();

            
        }

        public async Task<ExpenseCategory> CreateCategory(ExpenseCategory expenseCategory)
        {
            var AddedEntity = await _db.AddAsync(expenseCategory);
            await _db.SaveChangesAsync();
            var AddedCategory = AddedEntity.Entity;
            return AddedCategory;
        }

        public async Task<Travel> CreateTravel(Travel travel)
        {
            var AddedEntity = await _db.Travels.AddAsync(travel);
            await _db.SaveChangesAsync();
            var AddedTravel = AddedEntity.Entity;
            return AddedTravel;
        }

        public async Task DeleteTravel(int TravelId)
        {
            Travel travel = await GetTravelById(TravelId);
            travel.is_deleted = true;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistExpenseCategory(string category)
        {
            ExpenseCategory expenseCategory =await _db.ExpenseCategories.FirstOrDefaultAsync((c)=>c.Category.ToLower().Trim()  == category.ToLower().Trim());
            if(expenseCategory == null)
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

        public async Task<Travel> GetTravelById(int TravelId)
        {
            Travel? travel = await _db.Travels.
                Include(t => t.Travelers)
                    .ThenInclude(tr => tr.Travelerr)
                .Where(t => t.Id == TravelId)
                .FirstOrDefaultAsync();
            if (travel == null) {
                throw new NotFoundCustomException($"Travel With Id : {TravelId} Not Found !");
            }
            Travel t = travel;
            return t;
        }

        public async Task<PagedReponseOffSet<Travel>> GetTravelCreatedByHr(int HrId,int PageSize,int PageNumber)
        {
            var TotalRecords = await _db.Travels.Where(t => t.Id ==  HrId).CountAsync();
            List<Travel> Travels = await _db.Travels
                .OrderBy(t => t.Id)
                .Where(t => t.CreatedBy ==  HrId)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            PagedReponseOffSet<Travel> Response = new PagedReponseOffSet<Travel>(Travels, PageNumber, PageSize, TotalRecords);
            return Response;
        }

        public async Task<Travel> GetTravelerByTravel(int travelId)
        {
            Travel travel = await GetTravelById(travelId);
            return travel;
        }

        public async Task<Travel> UpdateTravel(Travel travel)
        {
            var AddedEntity = _db.Travels.Update(travel);
            await _db.SaveChangesAsync();
            var AddedTravel = AddedEntity.Entity;
            return AddedTravel;
        }

        public async Task<bool> UserExistsInTravel(int TravelId, int UserId)
        {
            Traveler response = await _db.Travelers.FirstOrDefaultAsync(t => t.TravelId == TravelId && t.TravelerId == UserId);
            if (response == null)
                return false;
            return true;
        }
    }
}
