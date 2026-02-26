using hrms.Dto.Response.DailyCelebration;
using hrms.Dto.Response.Game;
using hrms.Service;
using hrms.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace hrms.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController(IDailyCelebrationService _dailyCelebrationService, ILogger<DashboardController> _logger) : Controller
    {
        [HttpGet("daily-celebrations")]
        public async Task<IActionResult> GetDailyCelebrations()
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            List<DailyCelebrationResponseDto> celebrations = 
                    await _dailyCelebrationService.GetDailyCelebrationsForToday();
            _logger.LogInformation("[{Method}] {Url} - Fetched {Count} daily celebrations successfully", Request.Method, Request.Path, celebrations.Count);
            return Ok(celebrations);
        }

        [HttpGet("upcoming-bookings")]
        public async Task<IActionResult> GetUpcomingBookingSlots()
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            List<UpcomingBookingSlotResponseDto> bookingSlots = await _dailyCelebrationService.GetUpcomingBookingSlotsForToday();
            _logger.LogInformation("[{Method}] {Url} - Fetched {Count} upcoming booking slots successfully", Request.Method, Request.Path, bookingSlots.Count);
            return Ok(bookingSlots);
        }
    }
}
