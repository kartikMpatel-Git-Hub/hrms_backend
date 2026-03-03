using AutoMapper;
using hrms.dto.response.System;
using hrms.Repository;

namespace hrms.Service.impl
{
    public class SystemSettingsService(
        ISystemSettingsRepository _repo,
        IMapper _mapper,
        ICloudinaryService _cloudinaryService
        ) : ISystemSettingsService
    {
        public async Task<SystemSettingsResponseDto> GetSystemSettingsAsync()
        {
            var result = await _repo.GetSystemSettingsAsync();
            return _mapper.Map<SystemSettingsResponseDto>(result);
        }

        public async Task<SystemSettingsResponseDto> UpdateDefaultHrAsync(int defaultHrId, int updatedById)
        {
            var result = await _repo.UpdateDefaultHrAsync(defaultHrId, updatedById);
            return _mapper.Map<SystemSettingsResponseDto>(result);
        }

        public async Task<SystemSettingsResponseDto> UpdateBirthdayImageAsync(IFormFile file, int updatedById)
        {
            var imageUrl = await _cloudinaryService.UploadAsync(file);
            var result = await _repo.UpdateBirthdayImageAsync(imageUrl, updatedById);
            return _mapper.Map<SystemSettingsResponseDto>(result);
        }

        public async Task<SystemSettingsResponseDto> UpdateAnniversaryImageAsync(IFormFile file, int updatedById)
        {
            var imageUrl = await _cloudinaryService.UploadAsync(file);
            var result = await _repo.UpdateAnniversaryImageAsync(imageUrl, updatedById);
            return _mapper.Map<SystemSettingsResponseDto>(result);
        }

        public async Task<SystemSettingsResponseDto> UpdateDefaultProfileImageAsync(IFormFile file, int updatedById)
        {
            var imageUrl = await _cloudinaryService.UploadAsync(file);
            var result = await _repo.UpdateDefaultProfileImageAsync(imageUrl, updatedById);
            return _mapper.Map<SystemSettingsResponseDto>(result);
        }
    }
}