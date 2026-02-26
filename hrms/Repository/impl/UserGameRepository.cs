using hrms.Data;
using hrms.Dto.Response.Other;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Repository.impl
{
    public class UserGameRepository(ApplicationDbContext _db, ILogger<UserGameRepository> _logger) : IUserGameRepository
    {
        public async Task<UserGameInterest> CreateUserGameInterest(UserGameInterest userGameInterest)
        {
            var savedEntity = await _db.UserGameInterests.AddAsync(userGameInterest);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created UserGameInterest for UserId {UserId}, GameId {GameId}", userGameInterest.UserId, userGameInterest.GameId);
            return savedEntity.Entity;
        }

        public async Task<UserGameState> CreateUserState(UserGameState userGameState)
        {
            var savedEntity = await _db.UserGameStates.AddAsync(userGameState);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created UserGameState for UserId {UserId}, GameId {GameId}", userGameState.UserId, userGameState.GameId);
            return savedEntity.Entity;
        }

        public async Task<UserGameInterest> UpdateUserGameInterest(UserGameInterest userGameInterest)
        {
            var savedEntity = _db.UserGameInterests.Update(userGameInterest);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Updated UserGameInterest for UserId {UserId}, GameId {GameId}", userGameInterest.UserId, userGameInterest.GameId);
            return savedEntity.Entity;
        }

        public async Task<UserGameState> UpdateuserState(UserGameState userGameState)
        {
            var savedEntity = _db.UserGameStates.Update(userGameState);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Updated UserGameState for UserId {UserId}, GameId {GameId}", userGameState.UserId, userGameState.GameId);
            return savedEntity.Entity;
        }

        public async Task<UserGameState> GetUserGameState(int userId, int gameId)
        {
            UserGameState gameState = await _db.UserGameStates
                                .Where((ugs) => ugs.UserId == userId && ugs.GameId == gameId)
                                .FirstOrDefaultAsync();
            if (gameState == null)
            {
                gameState = new UserGameState()
                {
                    UserId = userId,
                    GameId = gameId,
                    GamePlayed = 0,
                    LastPlayedAt = DateTime.Now
                };
                var savedEntity = await _db.UserGameStates.AddAsync(gameState);
                await _db.SaveChangesAsync();
                return savedEntity.Entity;
            }
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

        public async Task<PagedReponseOffSet<User>> GetAvailablePlayers(int gameId, int currentUserId, string? key, int pageSize, int pageNumber)
        {
            int total = await _db.UserGameInterests
                            .Where(ugi => ugi.GameId == gameId &&
                                        ugi.Status == InterestStatus.INTERESTED &&
                                        ugi.UserId != currentUserId &&
                                        !ugi.User.is_deleted &&
                                        (ugi.User.FullName.Contains(key) || ugi.User.Email.Contains(key))
                                ).CountAsync();

            List<User> users = await _db.UserGameInterests
                            .Where(ugi => ugi.GameId == gameId &&
                                        ugi.Status == InterestStatus.INTERESTED &&
                                        ugi.UserId != currentUserId &&
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

        public async Task<List<int>> GetUserGameStates(int gameId, int gamePlayed)
        {

            // select * from user_game_interest t1
            // join user_game_state t2 on t1.GameId = t2.GameId and t1.UserId = t2.UserId
            // where t1.GameId = 1 and [status] = 'INTERESTED' and t2.game_played < 4
            // order by t2.game_played

            List<int> users = await _db.UserGameInterests
                    .Where(ugi => ugi.GameId == gameId &&
                                ugi.Status == InterestStatus.INTERESTED)
                    .Join(_db.UserGameStates,
                        ugi => new { ugi.GameId, ugi.UserId },
                        ugs => new { ugs.GameId, ugs.UserId },
                        (ugi, ugs) => new { ugi, ugs })
                    .Where(joined => joined.ugs.GamePlayed < gamePlayed)
                    .OrderBy(joined => joined.ugs.GamePlayed)
                    .Select(joined => joined.ugi.UserId)
                    .ToListAsync();
            return users;

            // List<int> users = await _db.UserGameStates
            //         .Where(ugs => ugs.GameId == gameId && ugs.GamePlayed < gamePlayed)
            //         .OrderBy(u => u.GamePlayed)
            //         .Select(u => u.UserId)
            //         .ToListAsync();
            // users = await _db.UserGameInterests
            //         .Where(
            //     (ugi) => ugi.GameId == gameId &&
            //             ugi.Status == InterestStatus.INTERESTED &&
            //             users.Contains(ugi.UserId)
            //     ).Select(ugi => ugi.UserId).ToListAsync();
            // return users;
        }

        public async Task<bool> ToggleGameInterestStatus(int userId, int gameId)
        {
            UserGameInterest? interest = _db.UserGameInterests
                .FirstOrDefault(ugi => ugi.UserId == userId && ugi.GameId == gameId);

            if (interest == null)
            {
                interest = new UserGameInterest
                {
                    UserId = userId,
                    GameId = gameId,
                    Status = InterestStatus.INTERESTED
                };
                await _db.UserGameInterests.AddAsync(interest);
            }
            else
            {
                interest.Status = interest.Status == InterestStatus.INTERESTED ? InterestStatus.NOTINTERESTED : InterestStatus.INTERESTED;
                _db.UserGameInterests.Update(interest);
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("Toggled game interest for UserId {UserId}, GameId {GameId}, interested: {Status}", userId, gameId, interest.Status == InterestStatus.INTERESTED);
            return interest.Status == InterestStatus.INTERESTED;
        }

        public async Task DecrementGamePlayed(int bookedById, int gameId)
        {
            UserGameState? gameState = await _db.UserGameStates.FirstOrDefaultAsync(ugs => ugs.UserId == bookedById && ugs.GameId == gameId);
            if (gameState != null && gameState.GamePlayed > 0)
            {
                gameState.GamePlayed -= 1;
                _db.UserGameStates.Update(gameState);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Decremented GamePlayed for UserId {UserId}, GameId {GameId}", bookedById, gameId);
            }
        }
    }
}