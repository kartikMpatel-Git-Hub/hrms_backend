using hrms.CustomException;
using hrms.Dto.Request.BookingSlot;
using hrms.Dto.Response.BookingSlot;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.User;
using hrms.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace hrms.Controllers
{
    [Route("game")]
    [ApiController]
    [Authorize]
    public class BookingSlotContoller(IGameSlotService _service) : Controller
    {

        [HttpPost("{gameId}/booking/{bookingSlotId}")]
        public async Task<IActionResult> BookSlot(int?
            bookingSlotId, BookSlotRequestDto dto)
        {
            if (bookingSlotId == null)
                return BadRequest($"booking slot id not found !");

            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);

            BookingSlotResponseDto response = await _service.BookSlot((int)bookingSlotId, CurrentUserId, dto);
            return Ok(response);
        }

        [HttpGet("{gameId}/booking/{bookingSlotId}")]
        public async Task<IActionResult> GetSlotInformation(int? bookingSlotId)
        {
            if (bookingSlotId == null)
                return BadRequest($"booking slot id not found !");
            BookingSlotWithPlayerResponseDto response = await _service.GetSlot((int)bookingSlotId);
            return Ok(response);
        }

        [HttpGet("{gameId}/booking")]
        public async Task<IActionResult> GetSlots(int? gameId
            ,DateTime? startDate, DateTime? endDate)
        {
            if (gameId == null)
                return BadRequest("Game id Not Found !");
            if (startDate == null)
            {
                startDate = DateTime.Now;
            }
            if (endDate == null)
            {
                endDate = DateTime.Now.AddDays(7);
            }
            if (startDate > endDate)
            {
                return BadRequest("Invalid Request For Slot !");
            }
            List<BookingSlotResponseDto> response = await _service.GetBookingSlots((int)gameId,(DateTime)startDate, (DateTime)endDate);
            return Ok(response);
        }

        [HttpGet("{gameId}/booking/available-players")]
        public async Task<IActionResult> GetAvailablePlayers(int? gameId, string key = "", int PageSize = 10, int PageNumber = 1)
        {
            if (gameId == null)
                return BadRequest("Game id Not Found !");
            PagedReponseDto<UserResponseDto> response = await _service.GetAvailablePlayers((int)gameId, key, PageSize, PageNumber);
            return Ok(response);
        }
    }
}
