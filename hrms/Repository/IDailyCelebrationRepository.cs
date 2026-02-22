using hrms.Model;

namespace hrms.Repository
{
    public interface IDailyCelebrationRepository
    {
        Task<DailyCelebration> AddDailyCelebration(DailyCelebration dailyCelebration);
        Task<List<User>> GetBirthdayUsersForToday();
        Task<List<DailyCelebration>> GetDailyCelebrationsForToday();
        Task<User> GetSystemUser();
        Task<List<User>> GetWorkAnniversaryUsersForToday();
        Task<bool> IsCelebrationAlreadyAdded(int id, DateTime now, EventType eventType);
    }
    
}