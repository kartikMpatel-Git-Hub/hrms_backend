using hrms.Dto.Response.DailyCelebration;

namespace hrms.Service
{
    public interface IDailyCelebrationService
    {
        Task AddDailyCelebration();
        Task<List<DailyCelebrationResponseDto>> GetDailyCelebrationsForToday();
    }
}