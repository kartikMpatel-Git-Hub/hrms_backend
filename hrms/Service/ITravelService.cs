using hrms.Dto.Request.Travel;
using hrms.Dto.Request.Travel.Document;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Travel;
using hrms.Dto.Response.Travel.Document;

namespace hrms.Service
{
    public interface ITravelService
    {
        Task<TravelResponseDto> CreateTravelAsync(TravelCreateDto Dto,int CurrentUser);
        Task<TravelResponseDto> GetTravelByIdAsync(int TravelId);
        Task<TravelWithTravelerResponseDto> GetTravelersByTravelId(int TravelId);
        Task<PagedReponseDto<TravelResponseDto>> GetHrCreatedTravels(int HrId, int PageSize, int PageNumber);
        Task RemoveTravel(int TravelId);
        Task<TravelResponseDto> UpdateTravelById(int TravelId,TravelUpdateDto dto);
        Task AddTraveler(int currentUserId, TravelerAddDto dto);
        Task<TravelDocumentResponseDto> AddTravelDocument(int travelId, int travelerId, int currentUserId, TravelDocumentCreateDto dto);
        Task<List<TravelDocumentResponseDto>> GetTravelDocument(int travelId, int travelerId);
        Task<PagedReponseDto<TravelResponseDto>> GetEmployeeTravels(int currentUserId, int pageSize, int pageNumber);
    }
}
