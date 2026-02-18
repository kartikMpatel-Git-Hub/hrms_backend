using hrms.Data;
using hrms.Dto.Response.Other;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Repository.impl
{
    public class UserGameRepository(ApplicationDbContext _db) : IUserGameRepository
    {
        public async Task<UserGameInterest> CreateUserGameInterest(UserGameInterest userGameInterest)
        {
            var savedEntity = await _db.UserGameInterests.AddAsync(userGameInterest);
            await _db.SaveChangesAsync();
            return savedEntity.Entity;
        }

        public async Task<UserGameState> CreateUserState(UserGameState userGameState)
        {
            var savedEntity = await _db.UserGameStates.AddAsync(userGameState);
            await _db.SaveChangesAsync();
            return savedEntity.Entity;
        }

        public async Task<UserGameInterest> UpdateUserGameInterest(UserGameInterest userGameInterest)
        {
            var savedEntity = _db.UserGameInterests.Update(userGameInterest);
            await _db.SaveChangesAsync();
            return savedEntity.Entity;
        }

        public async Task<UserGameState> UpdateuserState(UserGameState userGameState)
        {
            var savedEntity = _db.UserGameStates.Update(userGameState);
            await _db.SaveChangesAsync();
            return savedEntity.Entity;
        }

        public async Task<UserGameState> GetUserGameState(int userId, int gameId)
        {
            UserGameState gameState = await _db.UserGameStates
                                .Where((ugs) => ugs.UserId == userId && ugs.GameId == gameId)
                                .FirstOrDefaultAsync();
            return gameState;
        }

        public async Task<PagedReponseOffSet<User>> GetInterestedPlayers(int gameId, int pageNumber, int pageSize)
        {
            int total = await _db.UserGameInterests
                            .Where(
                                ugi => ugi.GameId == gameId &&
                                ugi.Status == InterestStatus.INTERESTED
                                ).CountAsync();
            List<User> users = await _db.UserGameInterests
                            .Where(ugi => ugi.GameId == gameId && ugi.Status == InterestStatus.INTERESTED)
                            .Select(ugi => ugi.User)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();
            PagedReponseOffSet<User> pagedReponse = new PagedReponseOffSet<User>(users, pageNumber, pageSize, total);
            return pagedReponse;
        }

        public async Task<PagedReponseOffSet<User>> GetInterestedPlayersByName(int gameId, string key, int pageNumber, int pageSize)
        {
            int total = await _db.UserGameInterests
                            .Where(
                                ugi => ugi.GameId == gameId &&
                                ugi.User.FullName.Contains(key) &&
                                ugi.User.Email.Contains(key) &&
                                ugi.Status == InterestStatus.INTERESTED
                                ).CountAsync();

            List<User> users = await _db.UserGameInterests
                            .Where(
                                ugi => ugi.GameId == gameId &&
                                ugi.User.FullName.Contains(key) &&
                                ugi.User.Email.Contains(key) &&
                                ugi.Status == InterestStatus.INTERESTED
                                )
                            .Select(ugi => ugi.User)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

            PagedReponseOffSet<User> pagedReponse = new PagedReponseOffSet<User>(users, pageNumber, pageSize, total);
            return pagedReponse;
        }

        public async Task<List<UserGameInterest>> GetInterestedPlayers(int gameId)
        {
            List<UserGameInterest> users = await _db.UserGameInterests
                            .Where(ugi => ugi.GameId == gameId && ugi.Status == InterestStatus.INTERESTED)
                            .ToListAsync();
            return users;
        }

        public async Task<List<UserGameState>> GetUsersGameStates(int gameId)
        {
            List<UserGameState> userStates = await _db.UserGameStates
                            .Where(ugs => ugs.GameId == gameId)
                            .ToListAsync();
            return userStates;
        }

        public Task<bool> IsUserInterestedInGame(int id, int gameId)
        {
            return _db.UserGameInterests
                    .Where(ugi => ugi.UserId == id && ugi.GameId == gameId && ugi.Status == InterestStatus.INTERESTED)
                    .AnyAsync();
        }

        public async Task<PagedReponseOffSet<User>> GetAvailablePlayers(int gameId, string? key, int pageSize, int pageNumber)
        {
            int total = await _db.UserGameInterests
                            .Where(ugi => ugi.GameId == gameId &&
                                        ugi.Status == InterestStatus.INTERESTED &&
                                        !ugi.User.is_deleted &&
                                        (ugi.User.FullName.Contains(key) || ugi.User.Email.Contains(key))
                                ).CountAsync();
            
            List<User> users = await _db.UserGameInterests
                            .Where(ugi => ugi.GameId == gameId &&
                                        ugi.Status == InterestStatus.INTERESTED &&
                                        !ugi.User.is_deleted &&
                                        (ugi.User.FullName.Contains(key) || ugi.User.Email.Contains(key))
                                )
                            .Select(ugi => ugi.User)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();  
            
            PagedReponseOffSet<User> pagedReponse = new PagedReponseOffSet<User>(users, pageNumber, pageSize, total);
            return pagedReponse;
        }
    }
}