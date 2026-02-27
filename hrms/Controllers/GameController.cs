using hrms.Dto.Request.BookingSlot;
using hrms.Dto.Request.Game;
using hrms.Dto.Response.Game;
using hrms.Dto.Response.Other;
using hrms.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hrms.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class GameController(IGameService _service, ILogger<GameController> _logger) : Controller
    {

        // game crud
        [HttpPost]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> CreateGame(GameCreateDto dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            GameResponseDto response = await _service.CreateGame(dto);
            _logger.LogInformation("[{Method}] {Url} - Game created successfully", Request.Method, Request.Path);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGames(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }
            PagedReponseDto<GameResponseDto> response = await _service.GetAllGames(pageNumber, pageSize);
            _logger.LogInformation("[{Method}] {Url} - Fetched {Count} games successfully", Request.Method, Request.Path, response.Data?.Count ?? 0);
            return Ok(response);
        }

        [HttpGet("{gameId}")]
        public async Task<IActionResult> GetGame(int? gameId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (gameId == null || gameId <= 0)
            {
                return BadRequest("Invalid game ID.");
            }
            GameResponseWithSlot response = await _service.GetGame((int)gameId);
            _logger.LogInformation("[{Method}] {Url} - Fetched game with ID {GameId} successfully", Request.Method, Request.Path, gameId);
            return Ok(response);
        }

        [HttpPut("{gameId}")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> UpdateGame(int? gameId, GameUpdateDto dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (gameId == null || gameId <= 0)
            {
                return BadRequest("Invalid game ID.");
            }
            GameResponseDto response = await _service.UpdateGame((int)gameId, dto);
            _logger.LogInformation("[{Method}] {Url} - Game with ID {GameId} updated successfully", Request.Method, Request.Path, gameId);
            return Ok(response);
        }

        [HttpDelete("{gameId}")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> DeleteGame(int? gameId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (gameId == null || gameId <= 0)
            {
                return BadRequest("Invalid game ID.");
            }
            bool response = await _service.DeleteGame((int)gameId);
            _logger.LogInformation("[{Method}] {Url} - Game with ID {GameId} deleted successfully", Request.Method, Request.Path, gameId);
            return Ok(new { message = response ? "Game deleted successfully." : "Failed to delete game." });
        }

        // game operation window crud
        [HttpPost("{gameId}/operation-window")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> CreateGameOperationWindow(int? gameId, GameOperationWindowCreateDto dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (gameId == null || gameId <= 0)
            {
                return BadRequest("Invalid game ID.");
            }
            GameOperationWindowResponseDto response = await _service.CreateGameOperationWindow((int)gameId, dto);
            _logger.LogInformation("[{Method}] {Url} - Operation window created for game {GameId} successfully", Request.Method, Request.Path, gameId);
            return Ok(response);
        }

        [HttpDelete("{gameId}/operation-window/{windowId}")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> DeleteGameOperationWindow(int? gameId, int? windowId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (gameId == null || gameId <= 0 || windowId == null || windowId <= 0)
            {
                return BadRequest("Invalid game ID or window ID.");
            }
            bool response = await _service.DeleteGameOperationWindow((int)gameId, (int)windowId);
            _logger.LogInformation("[{Method}] {Url} - Operation window {WindowId} deleted for game {GameId} successfully", Request.Method, Request.Path, windowId, gameId);
            return Ok(new { message = response ? "Game operation window deleted successfully." : "Failed to delete game operation window." });
        }

        [HttpGet("{gameId}/operation-window")]
        public async Task<IActionResult> GetAllGameOperationWindows(int? gameId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (gameId == null || gameId <= 0)
            {
                return BadRequest("Invalid game ID.");
            }
            List<GameOperationWindowResponseDto> response = await _service.GetAllGameOperationWindows((int)gameId);
            _logger.LogInformation("[{Method}] {Url} - Fetched {Count} operation windows for game {GameId} successfully", Request.Method, Request.Path, response.Count, gameId);
            return Ok(response);
        }

        // game slot crud

        [HttpGet("{gameId}/slots")]
        public async Task<IActionResult> GetAllGameSlots(int? gameId, DateTime? startDate = null, DateTime? endDate = null)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (gameId == null || gameId <= 0)
            {
                return BadRequest("Invalid game ID.");
            }
            if (startDate != null && endDate != null && startDate > endDate)
            {
                return BadRequest("Start date cannot be greater than end date.");
            }
            if ((startDate != null && endDate == null) || (startDate == null && endDate != null))
            {
                return BadRequest("Both start date and end date must be provided.");
            }
            if (startDate == null && endDate == null)
            {
                startDate = DateTime.Now.Date;
                endDate = DateTime.Now.Date.AddDays(7);
            }
            else if (startDate == null && endDate != null)
            {
                startDate = DateTime.Now.Date;
                if (endDate < startDate)
                {
                    endDate = startDate.Value.AddDays(7);
                }
            }
            else if (startDate != null && endDate == null)
            {
                endDate = startDate.Value.AddDays(7);
                if (endDate < startDate)
                {
                    return BadRequest("End date cannot be less than start date.");
                }
            }
            List<GameSlotResponseDto> response = await _service.GetAllGameSlots((int)gameId, startDate, endDate);
            _logger.LogInformation("[{Method}] {Url} - Fetched {Count} slots for game {GameId} successfully", Request.Method, Request.Path, response.Count, gameId);
            return Ok(response);
        }

        [HttpGet("{gameId}/slots/{slotId}/details")]
        public async Task<IActionResult> GetGameSlot(int? gameId, int? slotId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (gameId == null || gameId <= 0 || slotId == null || slotId <= 0)
            {
                return BadRequest("Invalid game ID or slot ID.");
            }
            GameSlotDetailResponseDto response = await _service.GetGameSlot((int)gameId, (int)slotId);
            _logger.LogInformation("[{Method}] {Url} - Fetched slot {SlotId} details for game {GameId} successfully", Request.Method, Request.Path, slotId, gameId);
            return Ok(response);
        }

        [HttpGet("{gameId}/slots/{slotId}/waitlist")]
        public async Task<IActionResult> GetGameSlotWaitlist(int? gameId, int? slotId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (gameId == null || gameId <= 0 || slotId == null || slotId <= 0)
            {
                return BadRequest("Invalid game ID or slot ID.");
            }
            List<GameSlotWaitinglistResponseDto> response = await _service.GetGameSlotWaitlist((int)gameId, (int)slotId);
            _logger.LogInformation("[{Method}] {Url} - Fetched {Count} waitlist entries for slot {SlotId} of game {GameId} successfully", Request.Method, Request.Path, response.Count, slotId, gameId);
            return Ok(response);
        }

        [HttpDelete("{gameId}/slots/{slotId}/waitlist/{waitlistId}")]
        public async Task<IActionResult> CancelGameSlotWaitlist(int? gameId, int? slotId, int? waitlistId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (gameId == null || gameId <= 0 || slotId == null || slotId <= 0 || waitlistId == null || waitlistId <= 0)
            {
                return BadRequest("Invalid game ID, slot ID, or waitlist ID.");
            }
            GameSlotWaitinglistResponseDto response = await _service.CancelWaitingListEntry((int)gameId, (int)slotId, (int)waitlistId);
            _logger.LogInformation("[{Method}] {Url} - Waitlist entry {WaitlistId} cancelled for slot {SlotId} of game {GameId} successfully", Request.Method, Request.Path, waitlistId, slotId, gameId);
            return Ok(response);
        }

        [HttpPost("{gameId}/slots/{slotId}")]
        public async Task<IActionResult> BookGameSlot(int? gameId, int? slotId, BookSlotRequestDto dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (gameId == null || gameId <= 0 || slotId == null || slotId <= 0)
            {
                return BadRequest("Invalid game ID or slot ID.");
            }
            var user = User;
            if (user == null)
                return Unauthorized("Unauthorized Access !");
            var userId = Int32.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid)?.Value);
            GameSlotResponseDto response = await _service.BookGameSlot((int)gameId, (int)slotId, userId, dto);
            _logger.LogInformation("[{Method}] {Url} - Slot {SlotId} of game {GameId} booked by user {UserId} successfully", Request.Method, Request.Path, slotId, gameId, userId);
            return Ok(response);
        }

        [HttpDelete("{gameId}/slots/{slotId}")]
        public async Task<IActionResult> CancelGameSlot(int? gameId, int? slotId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (gameId == null || gameId <= 0 || slotId == null || slotId <= 0)
            {
                return BadRequest("Invalid game ID or slot ID.");
            }
            GameSlotResponseDto response = await _service.CancelGameSlot((int)gameId, (int)slotId);
            _logger.LogInformation("[{Method}] {Url} - Slot {SlotId} of game {GameId} cancelled successfully", Request.Method, Request.Path, slotId, gameId);
            return Ok(response);
        }
        [HttpDelete("{gameId}/slots/{slotId}/delete")]
        public async Task<IActionResult> DeleteGameSlot(int? gameId, int? slotId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (gameId == null || gameId <= 0 || slotId == null || slotId <= 0)
            {
                return BadRequest("Invalid game ID or slot ID.");
            }
            await _service.DeleteGameSlot((int)gameId, (int)slotId);
            _logger.LogInformation("[{Method}] {Url} - Slot {SlotId} of game {GameId} deleted successfully", Request.Method, Request.Path, slotId, gameId);
            return Ok(new { message = "Game slot deleted successfully." });
        }

        [HttpPatch("{gameId}/toggle-interest")]
        public async Task<IActionResult> ToggleGameInterest(int? gameId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (gameId == null || gameId <= 0)
            {
                return BadRequest("Invalid game ID.");
            }
            var user = User;
            if (user == null)
                return Unauthorized("Unauthorized Access !");
            var userId = Int32.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid)?.Value);
            bool response = await _service.ToggleGameInterest((int)gameId, userId);
            _logger.LogInformation("[{Method}] {Url} - Game interest toggled for game {GameId} by user {UserId} successfully", Request.Method, Request.Path, gameId, userId);
            return Ok(new { message = response ? "Game interest toggled successfully." : "Failed to toggle game interest." });
        }

        [HttpGet("{gameId}/toggle-interest")]
        public async Task<IActionResult> IsUserInterested(int? gameId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (gameId == null || gameId <= 0)
            {
                return BadRequest("Invalid game ID.");
            }
            var user = User;
            if (user == null)
                return Unauthorized("Unauthorized Access !");
            var userId = Int32.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid)?.Value);
            bool response = await _service.IsUserInterested((int)gameId, (int)userId);
            _logger.LogInformation("[{Method}] {Url} - Checked interest in game {GameId} for user {UserId} successfully", Request.Method, Request.Path, gameId, userId);
            return Ok(new { isInterested = response });
        }
    }
}
