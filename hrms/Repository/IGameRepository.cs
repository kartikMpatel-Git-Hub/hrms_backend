using hrms.Dto.Response.Other;
using hrms.Model;

namespace hrms.Repository
{
    public interface IGameRepository
    {
        Task<Game> CreateGame(Game newGame);
        Task<GameSlot> CreateGameSlot(GameSlot newSlot);
        Task<PagedReponseOffSet<Game>> GetAllGames(int pageNumber, int pageSize);
        Task<List<Game>> GetAllGames();
        Task<Game> GetGameById(int gameId);
        Task<List<GameSlot>> GetGameSlots(int gameId);
        Task<GameSlot> GetGameSlotById(int gameSlotId);
        Task<bool> isSlotExist(TimeOnly startTime, TimeOnly endTime);
        Task RemoveGame(Game game);
        Task RemoveGameSlot(GameSlot gameSlot);
        Task<Game> UpdateGame(Game updatedGame);
        Task<GameSlot> UpdateGameSlot(GameSlot updatedGameSlot);
        Task CreateBookingSlot(BookingSlot bookingSlot);
    }
}
