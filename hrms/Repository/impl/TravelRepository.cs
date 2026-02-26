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
        private readonly ILogger<TravelRepository> _logger;

        public TravelRepository(ApplicationDbContext db, ILogger<TravelRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<TravelDocument> AddTravelDocument(TravelDocument document)
        {
            var AddedEntity = await _db.TravelDocuments.AddAsync(document);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Added TravelDocument with Id {Id} for TravelId {TravelId}", AddedEntity.Entity.Id, document.TravelId);
            return AddedEntity.Entity;
        }

        public async Task<Traveler> AddTraveler(Traveler traveler)
        {
            var AddedEntity= await _db.Travelers.AddAsync(traveler);
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
            _logger.LogInformation("Added Traveler with Id {Id} to TravelId {TravelId}", AddedEntity.Entity.Id, traveler.TravelId);
            return AddedEntity.Entity;
        }

        public async Task<Travel> CreateTravel(Travel travel)
        {
            var AddedEntity = await _db.Travels.AddAsync(travel);
            await _db.SaveChangesAsync();
            var AddedTravel = AddedEntity.Entity;
            _logger.LogInformation("Created Travel with Id {Id}", AddedTravel.Id);
            return AddedTravel;
        }

        public async Task DeleteTravel(int TravelId)
        {
            Travel travel = await GetTravelById(TravelId);
            travel.is_deleted = true;
            await _db.SaveChangesAsync();
            _logger.LogInformation("Soft-deleted Travel with Id {Id}", TravelId);
        }

        public async Task<PagedReponseOffSet<TravelDocument>> GetDocumentsByTravelIdAndTravelerId(int travelId, int travelerId, int pageSize, int pageNumber)
        {
            int TotalRecords = await _db.TravelDocuments.Where(td => td.TravelId == travelId && td.TravelerId == travelerId).CountAsync();
            List<TravelDocument> documents = await _db.TravelDocuments
                .Where(td => td.TravelId == travelId && td.TravelerId == travelerId)
                .Include(td => td.Uploader)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(td => td.UploadedAt)
                .ToListAsync();
            PagedReponseOffSet<TravelDocument> Response = new PagedReponseOffSet<TravelDocument>(documents, pageNumber, pageSize, TotalRecords);
            return Response;
        }

        public async Task<PagedReponseOffSet<Travel>> GetEmployeeTravels(int id, int pageSize, int pageNumber)
        {
            var TotalRecords = await _db
                .Travelers
                .Where(tl => tl.TravelerId == id)
                .CountAsync();
            List<Travel> travels = await _db
                .Travelers
                .Where(tl => tl.TravelerId == id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(tl => tl.Travel)
                .Select(tl => tl.Travel)
                .OrderByDescending(t => t.created_at)
                .ToListAsync();
            PagedReponseOffSet<Travel> Response = new PagedReponseOffSet<Travel>(travels, pageNumber, pageSize, TotalRecords);
            return Response;
        }

        public async Task<PagedReponseOffSet<Expense>> GetExpensesByTravelIdAndTravelerId(int travelId, int travelerId, int pageSize, int pageNumber)
        {
            int TotalRecords = await _db.Expenses.Where(e => e.TravelId == travelId && e.TravelerId == travelerId).CountAsync();
            List<Expense> expenses = await _db.Expenses
                .OrderByDescending(e => e.ExpenseDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Where(e => e.TravelId == travelId && e.TravelerId == travelerId)
                .Include(e => e.Proofs)
                .Include(e => e.Category)
                .ToListAsync();
            PagedReponseOffSet<Expense> Response = new PagedReponseOffSet<Expense>(expenses, pageNumber, pageSize, TotalRecords);
            return Response;
        }

        public async Task<decimal> GetTodaysExpense(int travelId, int currentUserId,DateTime dateTime)
        {
            List<Expense> expenses = await _db.Expenses
                .Where(e => e.TravelId == travelId && 
                e.TravelerId == currentUserId && 
                e.Status != ExpenseStatus.REJECTED &&
                e.ExpenseDate.Date == dateTime.Date)
                .ToListAsync();
            decimal total = 0;
            foreach(var expense in expenses)
            {
                total += expense.Amount;
            }
            return total;
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
            _logger.LogInformation("Fetched Travel with Id {Id}", TravelId);
            Travel t = travel;
            return t;
        }

        public async Task<PagedReponseOffSet<Travel>> GetTravelCreatedByHr(int HrId,int PageSize,int PageNumber)
        {
            var TotalRecords = await _db.Travels.Where(t => t.CreatedBy ==  HrId).CountAsync();
            List<Travel> Travels = await _db.Travels
                .Where(t => t.CreatedBy ==  HrId)
                .OrderByDescending(t => t.created_at)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            PagedReponseOffSet<Travel> Response = new PagedReponseOffSet<Travel>(Travels, PageNumber, PageSize, TotalRecords);
            return Response;
        }

        public async Task<List<TravelDocument>> GetTravelDocuments(int travelId, int travelerId,int userId)
        {
            List<TravelDocument> documents =
                await _db.TravelDocuments
                .Where((td) => td.TravelId == travelId && td.TravelerId == travelerId)
                .Include(td => td.Uploader)
                .ToListAsync();
            if (documents == null)
                throw new NotFoundCustomException("Document not found !");
            return documents;
        }

        public async Task<Travel> GetTravelerByTravel(int travelId)
        {
            Travel travel = await GetTravelById(travelId);
            return travel;
        }

        public async Task<User> GetTravelerByTravelId(int travelId, int travelerId)
        {
            Traveler traveler = await _db.Travelers
                .Where((t)=>t.TravelerId == travelerId && t.TravelId == travelId)
                .FirstOrDefaultAsync();
            
            if(traveler == null)
            {
                throw new NotFoundCustomException($"Traveler with {travelerId} not asociate with travel {travelId}");
            }

            User user = traveler.Travelerr;
            if (user == null)
                throw new InvalidOperationCustomException("Failed To fetch traveler !");
            return user;

        }

        public async Task<PagedReponseOffSet<Travel>> GetTravelsByTravelerId(int travelerId, int pageSize, int pageNumber)
        {
            int TotalRecords = await _db.Travels
                .Where(t => !t.is_deleted && t.Travelers.Any(tr => tr.TravelerId == travelerId))
                .Include(t => t.Travelers.Where(tr => tr.TravelerId == travelerId))
                .CountAsync();
            List<Travel> Travels = await _db.Travels
                .Where(t => !t.is_deleted && t.Travelers.Any(tr => tr.TravelerId == travelerId))
                .OrderByDescending(t => t.created_at)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(t => t.Travelers.Where(tr => tr.TravelerId == travelerId))
                .ToListAsync();
            PagedReponseOffSet<Travel> Response = new PagedReponseOffSet<Travel>(Travels, pageNumber, pageSize, TotalRecords);
            return Response;    
        }

        public async Task<Travel> UpdateTravel(Travel travel)
        {
            travel.updated_at = DateTime.Now;
            var AddedEntity = _db.Travels.Update(travel);
            await _db.SaveChangesAsync();
            var AddedTravel = AddedEntity.Entity;
            _logger.LogInformation("Updated Travel with Id {Id}", travel.Id);
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
