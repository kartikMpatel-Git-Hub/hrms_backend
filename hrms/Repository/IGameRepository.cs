using hrms.Dto.Request.BookingSlot;
using hrms.Dto.Response.Game;
using hrms.Dto.Response.Other;
using hrms.Model;

namespace hrms.Repository
{
    public interface IGameRepository
    {
        Task<GameSlot> BookGameSlot(int gameId, int slotId, int userId, BookSlotRequestDto dto);
        Task<Game> CreateGame(Game game);
        Task<GameOperationWindow> CreateGameOperationWindow(GameOperationWindow window);
        Task DeleteGame(Game game);
        Task DeleteGameOperationWindow(GameOperationWindow window);
        Task DeleteGameSlot(GameSlot slot);
        Task<List<GameOperationWindow>> GetAllGameOperationWindows(int gameId);
        Task<PagedReponseOffSet<Game>> GetAllGames(int pageNumber, int pageSize);
        Task<List<GameSlot>> GetAllGameSlots(int gameId, DateTime startDate, DateTime endDate);
        Task<Game> GetGameById(int gameId);
        Task<GameOperationWindow> GetGameOperationWindowById(int windowId);
        Task<GameSlot> GetGameSlotById(int gameId, int slotId);
        Task<List<GameSlotWaiting>> GetGameSlotWaitlist(int gameId, int slotId);
        Task<GameSlotWaiting> GetGameSlotWaitlistById(int waitlistId);
        Task<List<GameSlot>> GetUpcomingBookingSlotsForToday();
        Task<GameSlotWaiting> GetUserWaitlistEntryForSlot(int gameId, int slotId, int bookedById);
        Task<bool> IsOperationWindowOverlap(int gameId, TimeOnly operationalStartTime, TimeOnly operationalEndTime);
        Task<bool> IsUserAlreadyBookedInSlot(int gameId, int slotId, int userId);
        Task RemovePlayerFromGameSlot(int id);
        Task<Game> UpdateGame(Game updatedGame);
        Task<GameSlot> UpdateGameSlot(GameSlot slot);
        Task UpdateGameSlotWaiting(GameSlotWaiting myWaitlistEntry);
    }
}
