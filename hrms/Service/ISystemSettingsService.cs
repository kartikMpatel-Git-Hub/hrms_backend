using hrms.dto.response.System;

namespace hrms.Service
{
    public interface ISystemSettingsService
    {
        Task<SystemSettingsResponseDto> GetSystemSettingsAsync();
        Task<SystemSettingsResponseDto> UpdateDefaultHrAsync(int defaultHrId, int updatedById);
        Task<SystemSettingsResponseDto> UpdateBirthdayImageAsync(IFormFile file, int updatedById);
        Task<SystemSettingsResponseDto> UpdateAnniversaryImageAsync(IFormFile file, int updatedById);
        Task<SystemSettingsResponseDto> UpdateDefaultProfileImageAsync(IFormFile file, int updatedById);
    }
}