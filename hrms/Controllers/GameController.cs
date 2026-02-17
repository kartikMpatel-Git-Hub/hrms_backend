using hrms.Dto.Request.Game;
using hrms.Dto.Request.Game.GameSlot;
using hrms.Dto.Response.Game;
using hrms.Dto.Response.Game.GameSlot;
using hrms.Dto.Response.Other;
using hrms.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hrms.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class GameController : Controller
    {
        private readonly IGameService _service;
        public GameController(IGameService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> CreateGame(GameCreateDto? dto)
        {
            if (dto == null)
                return BadRequest("Game create body not found !");
            GameResponseDto response = await _service.CreateGame(dto);
            return Ok(response);
        }

        [HttpPost("{gameId}/slot")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> CreateGameSlote(int? gameId, GameSlotCreateDto? dto)
        {
            if (gameId == null)
                return BadRequest($"Game Id not found !");
            if(dto == null)
                return BadRequest("Game slot create body not found !");
            GameSlotResponseDto response = await _service.CreateGameSlot((int)gameId, dto);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGames(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Invalid Request for data");
            }
            PagedReponseDto<GameResponseDto> response = await _service.GetAllGames(pageNumber, pageSize);
            return Ok(response);
        }

        [HttpGet("{gameId}")]
        public async Task<IActionResult> GetGame(int? gameId)
        {
            if (gameId == null)
                return BadRequest("Game Id not found !");
            GameResponseWithSlot response = await _service.GetGame((int)gameId);

            return Ok(response);
        }

        [HttpPut("{gameId}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> UpdateGame(int? gameId, GameUpdateDto? dto)
        {
            if (gameId == null)
                return BadRequest("Game Id not found !");
            if (dto == null)
                return BadRequest("Game Update Body not found !");
            GameResponseDto response = await _service.UpdateGame((int)gameId, (GameUpdateDto)dto);
            return Ok(response);
        }

        [HttpPut("{gameId}/slot/{gameSlotId}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> UpdateGameSlot(int? gameSlotId, GameSlotUpdateDto? dto)
        {
            if (gameSlotId == null)
                return BadRequest("Game slot Id not found !");
            if (dto == null)
                return BadRequest("Game slot Update Body not found !");
            GameSlotResponseDto response = await _service.UpdateGameSlot((int)gameSlotId, (GameSlotUpdateDto)dto);
            return Ok(response);
        }

        [HttpDelete("{gameId}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> DeleteGame(int? gameId)
        {
            if (gameId == null)
                return BadRequest("Game Id not found !");
            await _service.DeleteGame((int)gameId);
            return Ok(new { message = $"game with id : {gameId} deleted !" });
        }


        [HttpDelete("{gameId}/slot/{gameSlotId}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> DeleteGameSlot(int? gameSlotId)
        {
            if (gameSlotId == null)
                return BadRequest("Game Id not found !");
            await _service.DeleteGameSlot((int)gameSlotId);
            return Ok(new { message = $"game slot with id : {gameSlotId} deleted !" });
        }
    }
}
