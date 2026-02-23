using hrms.Dto.Response.DailyCelebration;
using hrms.Dto.Response.Game;
using hrms.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hrms.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController(IDailyCelebrationService _dailyCelebrationService) : Controller
    {
        [HttpGet("daily-celebrations")]
        public async Task<IActionResult> GetDailyCelebrations()
        {
            List<DailyCelebrationResponseDto> celebrations =await _dailyCelebrationService.GetDailyCelebrationsForToday();
            return Ok(celebrations);
        }

        [HttpGet("upcoming-bookings")]
        public async Task<IActionResult> GetUpcomingBookingSlots()
        {
            List<UpcomingBookingSlotResponseDto> bookingSlots = await _dailyCelebrationService.GetUpcomingBookingSlotsForToday();
            return Ok(bookingSlots);
        }
    }
}
