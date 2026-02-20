using hrms.Dto.Request.Game;
using hrms.Dto.Request.Game.GameSlot;
using hrms.Dto.Response.Game;
using hrms.Dto.Response.Game.GameSlot;
using hrms.Dto.Response.Other;

namespace hrms.Service
{
    public interface IGameService
    {
        Task<GameResponseDto> CreateGame(GameCreateDto dto);
        Task<GameSlotResponseDto> CreateGameSlot(int gameId, GameSlotCreateDto dto);
        Task DeleteGame(int gameId);
        Task DeleteGameSlot(int gameSlotId);
        Task<PagedReponseDto<GameResponseDto>> GetAllGames(int pageNumber, int pageSize);
        Task<GameResponseWithSlot> GetGame(int gameId);
        Task<bool> IsUserInterestedInGame(int currentUserId, int gameId);
        Task<bool> ToggleGameInterestStatus(int currentUserId, int gameId);
        Task<GameResponseDto> UpdateGame(int gameId, GameUpdateDto dto);
        Task<GameSlotResponseDto> UpdateGameSlot(int gameSlotId, GameSlotUpdateDto dto);
    }
}
