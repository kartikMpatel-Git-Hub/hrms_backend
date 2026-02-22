using hrms.Dto.Response.Other;
using hrms.Model;

namespace hrms.Repository
{
    public interface ITravelRepository
    {
        Task<Travel> CreateTravel(Travel travel);
        Task<bool> UserExistsInTravel(int TravelId, int UserId);
        Task<Traveler> AddTraveler(Traveler traveler);
        Task<Travel> UpdateTravel(Travel travel);
        Task<Travel> GetTravelById(int TravelId);
        Task DeleteTravel(int TravelId);
        Task<PagedReponseOffSet<Travel>> GetTravelCreatedByHr(int HrId,int PageSize,int PageNumber);
        Task<Travel> GetTravelerByTravel(int travelId);
        Task<TravelDocument> AddTravelDocument(TravelDocument document);
        Task<User> GetTravelerByTravelId(int travelId, int travelerId);
        Task<List<TravelDocument>> GetTravelDocuments(int travelId, int travelerId, int userId);
        Task<PagedReponseOffSet<Travel>> GetEmployeeTravels(int id, int pageSize, int pageNumber);
        Task<decimal> GetTodaysExpense(int travelId, int currentUserId, DateTime dateTime);
        Task<PagedReponseOffSet<Travel>> GetTravelsByTravelerId(int travelerId, int pageSize, int pageNumber);
    }
}
