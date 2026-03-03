using hrms.Model;

namespace hrms.Repository
{
    public interface ISystemSettingsRepository
    {
        Task<SystemSettings> GetSystemSettingsAsync();
        Task<SystemSettings> UpdateDefaultHrAsync(int defaultHrId, int updatedById);
        Task<SystemSettings> UpdateBirthdayImageAsync(string imageUrl, int updatedById);
        Task<SystemSettings> UpdateAnniversaryImageAsync(string imageUrl, int updatedById);
        Task<SystemSettings> UpdateDefaultProfileImageAsync(string imageUrl, int updatedById);
    }
}