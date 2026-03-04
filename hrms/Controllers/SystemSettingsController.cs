using hrms.Dto.Request.System;
using hrms.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace hrms.Controllers
{
    [Route("system-settings")]
    [ApiController]
    [Authorize(Roles = "ADMIN, HR")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class SystemSettingsController(ISystemSettingsService _systemSettingsService) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetSystemSettings()
        {
            var settings = await _systemSettingsService.GetSystemSettingsAsync();
            return Ok(settings);
        }

        [HttpPut("default-hr")]
        public async Task<IActionResult> UpdateDefaultHr(UpdateDefaultHrRequestDto dto)
        {
            int userId = Int32.Parse(User.FindFirst(ClaimTypes.PrimarySid)?.Value);
            var settings = await _systemSettingsService.UpdateDefaultHrAsync(dto.DefaultHrId, userId);
            return Ok(settings);
        }

        [HttpPut("birthday-image")]
        public async Task<IActionResult> UpdateBirthdayImage(IFormFile BirthdayImage)
        {
            int userId = Int32.Parse(User.FindFirst(ClaimTypes.PrimarySid)?.Value);
            var settings = await _systemSettingsService.UpdateBirthdayImageAsync(BirthdayImage, userId);
            return Ok(settings);
        }

        [HttpPut("anniversary-image")]
        public async Task<IActionResult> UpdateAnniversaryImage(IFormFile AnniversaryImage)
        {
            int userId = Int32.Parse(User.FindFirst(ClaimTypes.PrimarySid)?.Value);
            var settings = await _systemSettingsService.UpdateAnniversaryImageAsync(AnniversaryImage, userId);
            return Ok(settings);
        }

        [HttpPut("default-profile-image")]
        public async Task<IActionResult> UpdateDefaultProfileImage(IFormFile DefaultProfileImage)
        {
            int userId = Int32.Parse(User.FindFirst(ClaimTypes.PrimarySid)?.Value);
            var settings = await _systemSettingsService.UpdateDefaultProfileImageAsync(DefaultProfileImage, userId);
            return Ok(settings);
        }
    }
}