using hrms.CustomException;
using hrms.Dto.Request.BookingSlot;
using hrms.Dto.Response.BookingSlot;
using hrms.Dto.Response.Game.offere;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.User;
using hrms.Model;
using hrms.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace hrms.Controllers
{
    [Route("game")]
    [ApiController]
    [Authorize]
    public class BookingSlotContoller(
        IGameSlotService _service,
        ISlotBookingService _slotBookingService
        ) : Controller
    {

        [HttpPost("{gameId}/slot/{bookingSlotId}/book")]
        public async Task<IActionResult> BookSlot(int?
            bookingSlotId, BookSlotRequestDto dto)
        {
            if (bookingSlotId == null)
                return BadRequest($"booking slot id not found !");

            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            if(dto == null || dto.Players == null || dto.Players.Count == 0)
                return BadRequest($"Players Information Not Found !");
            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);

            BookingSlotResponseDto response = await _slotBookingService.BookSlot((int)bookingSlotId, CurrentUserId, dto);
            return Ok(response);
        }

        [HttpPost("offere/{offerId}/accept")]
        public async Task<IActionResult> AcceptOffere(int? offerId,BookSlotRequestDto? dto)
        {
            if (offerId == null)
                return BadRequest($"Offere Id Not Found !");
            if(dto == null || dto.Players == null || dto.Players.Count == 0)
                return BadRequest($"Players Information Not Found !");
            await _slotBookingService.AcceptOffer((int)offerId, dto);
            return Ok($"Offere Accepted !");
        }

        [HttpPost("offere/{offerId}/reject")]
        public async Task<IActionResult> RejectOffere(int? offerId)
        {
            if (offerId == null)
                return BadRequest($"Offere Id Not Found !");
            await _slotBookingService.RejectOffer((int)offerId);
            return Ok($"Offere Rejected !");
        }

        [HttpGet("{gameId}/offere")]
        public async Task<IActionResult> GetActiveOfferes(int? gameId)
        {
            if (gameId == null)
                return BadRequest("Game id Not found !!");
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            List<SlotOffereResponseDto> slotOfferes = await _slotBookingService.GetActiveOfferes(CurrentUserId,(int)gameId);
            return Ok(slotOfferes);
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
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<UserResponseDto> response = await _service.GetAvailablePlayers((int)gameId, CurrentUserId, key, PageSize, PageNumber);
            return Ok(response);
        }
    }
}
