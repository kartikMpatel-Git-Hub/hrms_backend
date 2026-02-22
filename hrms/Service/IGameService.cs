using hrms.Dto.Request.BookingSlot;
using hrms.Dto.Request.Game;
using hrms.Dto.Response.Game;
using hrms.Dto.Response.Other;

namespace hrms.Service
{
    public interface IGameService
    {
        Task<GameSlotResponseDto> BookGameSlot(int gameId, int slotId, int userId, BookSlotRequestDto dto);
        Task<GameSlotResponseDto> CancelGameSlot(int gameId, int slotId);
        Task<GameSlotWaitinglistResponseDto> CancelWaitingListEntry(int gameId, int slotId, int waitlistId);
        Task<GameResponseDto> CreateGame(GameCreateDto dto);
        Task<GameOperationWindowResponseDto> CreateGameOperationWindow(int gameId, GameOperationWindowCreateDto dto);
        Task<bool> DeleteGame(int gameId);
        Task<bool> DeleteGameOperationWindow(int gameId, int windowId);
        Task<List<GameOperationWindowResponseDto>> GetAllGameOperationWindows(int gameId);
        Task<PagedReponseDto<GameResponseDto>> GetAllGames(int pageNumber, int pageSize);
        Task<List<GameSlotResponseDto>> GetAllGameSlots(int gameId, DateTime? startDate, DateTime? endDate);
        Task<GameResponseWithSlot> GetGame(int gameId);
        Task<GameSlotDetailResponseDto> GetGameSlot(int gameId, int slotId);
        Task<List<GameSlotWaitinglistResponseDto>> GetGameSlotWaitlist(int gameId, int slotId);
        Task<bool> IsUserInterested(int gameId, int userId);
        Task<bool> ToggleGameInterest(int gameId, int userId);
        Task<GameResponseDto> UpdateGame(int gameId, GameUpdateDto dto);
    }
}
