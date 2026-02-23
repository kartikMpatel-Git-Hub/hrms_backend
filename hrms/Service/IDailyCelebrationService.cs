using hrms.Dto.Response.DailyCelebration;
using hrms.Dto.Response.Game;

namespace hrms.Service
{
    public interface IDailyCelebrationService
    {
        Task AddDailyCelebration();
        Task<List<DailyCelebrationResponseDto>> GetDailyCelebrationsForToday();
        Task<List<UpcomingBookingSlotResponseDto>> GetUpcomingBookingSlotsForToday();
    }
}