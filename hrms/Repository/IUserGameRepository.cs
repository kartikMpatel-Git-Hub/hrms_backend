using hrms.Dto.Response.Other;
using hrms.Model;

namespace hrms.Repository
{
    public interface IUserGameRepository
    {
        Task<UserGameState> CreateUserState(UserGameState userGameState);
        Task<UserGameState> UpdateuserState(UserGameState userGameState);
        Task<UserGameInterest> CreateUserGameInterest(UserGameInterest userGameInterest);
        Task<UserGameInterest> UpdateUserGameInterest(UserGameInterest userGameInterest);
        Task<UserGameState> GetUserGameState(int userId, int gameId);
        Task<PagedReponseOffSet<User>> GetInterestedPlayers(int gameId, int pageNumber, int pageSize);
        Task<PagedReponseOffSet<User>> GetInterestedPlayersByName(int gameId, string key, int pageNumber, int pageSize);
        Task<List<UserGameInterest>> GetInterestedPlayers(int gameId);
        Task<List<UserGameState>> GetUsersGameStates(int gameId);
        Task<bool> IsUserInterestedInGame(int id, int gameId);
        Task<PagedReponseOffSet<User>> GetAvailablePlayers(int gameId, string? key, int pageSize, int pageNumber);
    }
}
