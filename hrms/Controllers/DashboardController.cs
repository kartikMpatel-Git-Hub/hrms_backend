using hrms.Dto.Response.DailyCelebration;
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
    }
}
