using hrms.CustomException;
using hrms.Data;
using hrms.Dto.Request.BookingSlot;
using hrms.Dto.Response.Game;
using hrms.Dto.Response.Other;
using hrms.Model;
using Microsoft.EntityFrameworkCore;
namespace hrms.Repository.impl
{
    public class GameRepository(ApplicationDbContext _db) : IGameRepository
    {
        public async Task<GameSlot> BookGameSlot(int gameId, int slotId, int userId, BookSlotRequestDto dto)
        {
            var waiting = new GameSlotWaiting();
            waiting.GameSlotId = slotId;
            waiting.RequestedById = userId;
            waiting.RequestedAt = DateTime.Now;
            await _db.GameSlotWaitings.AddAsync(waiting);
            await _db.SaveChangesAsync();

            foreach (var player in dto.Players)
            {
                var waitingPlayer = new GameSlotWaitingPlayer
                {
                    GameSlotWaitingId = waiting.Id,
                    PlayerId = player
                };
                await _db.GameSlotWaitingPlayers.AddAsync(waitingPlayer);
            }
            GameSlot gameSlot = await _db.GameSlots
                                .FirstOrDefaultAsync(s => s.Id == slotId && s.GameId == gameId);
            if (gameSlot == null)
            {
                throw new NotFoundCustomException($"Game Slot with ID {slotId} for Game ID {gameId} not found.");
            }
            gameSlot.Status = GameSlotStatus.WAITING;
            _db.GameSlots.Update(gameSlot);
            await _db.SaveChangesAsync();
            return gameSlot;
        }

        public async Task<Game> CreateGame(Game game)
        {
            var createdGame = await _db.Games.AddAsync(game);
            await _db.SaveChangesAsync();
            return createdGame.Entity;
        }

        public async Task<GameOperationWindow> CreateGameOperationWindow(GameOperationWindow window)
        {
            var createdWindow = await _db.GameOperationWindows.AddAsync(window);
            await _db.SaveChangesAsync();
            return createdWindow.Entity;
        }

        public async Task DeleteGame(Game game)
        {
            game.is_deleted = true;
            _db.Games.Update(game);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteGameOperationWindow(GameOperationWindow window)
        {
            window.is_deleted = true;
            _db.GameOperationWindows.Update(window);
            await _db.SaveChangesAsync();
        }

        public async Task<List<GameOperationWindow>> GetAllGameOperationWindows(int gameId)
        {
            return await _db.GameOperationWindows
                            .Where(w => w.GameId == gameId && !w.is_deleted)
                            .ToListAsync();
        }

        public async Task<PagedReponseOffSet<Game>> GetAllGames(int pageNumber, int pageSize)
        {
            int totalRecords = await _db.Games.CountAsync(g => !g.is_deleted);
            List<Game> games = await _db.Games
                                        .Where(g => !g.is_deleted)
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();
            return new PagedReponseOffSet<Game>(games, totalRecords, pageNumber, pageSize);
        }

        public async Task<List<GameSlot>> GetAllGameSlots(int gameId, DateTime startDate, DateTime endDate)
        {
            List<GameSlot> query = await _db.GameSlots
                        .Where(s => s.GameId == gameId && s.Date >= startDate.Date && s.Date <= endDate.Date)
                        .OrderBy(s => s.Date).ThenBy(s => s.StartTime)
                        .ToListAsync();
            return query;
        }

        public async Task<Game> GetGameById(int gameId)
        {
            Game game = await _db
                        .Games
                        .Include(g => g.GameOperationWindows.Where(w => !w.is_deleted).OrderBy(w => w.OperationalStartTime))
                        .FirstOrDefaultAsync(g => g.Id == gameId && !g.is_deleted);
            if (game == null)
            {
                throw new NotFoundCustomException($"Game with ID {gameId} not found.");
            }
            return game;
        }

        public async Task<GameOperationWindow> GetGameOperationWindowById(int windowId)
        {
            GameOperationWindow window = await _db.GameOperationWindows.FirstOrDefaultAsync(w => w.Id == windowId && !w.is_deleted);
            if (window == null)
            {
                throw new NotFoundCustomException($"Game Operation Window with ID {windowId} not found.");
            }
            return window;
        }

        public async Task<GameSlot> GetGameSlotById(int gameId, int slotId)
        {
            GameSlot slot = await _db.GameSlots
                    .Include(gs => gs.BookedBy)
                    .Include(gs => gs.Players).ThenInclude(p => p.Player)
                    .FirstOrDefaultAsync(s => s.Id == slotId && s.GameId == gameId);
            if (slot == null)
            {
                throw new NotFoundCustomException($"Game Slot with ID {slotId} for Game ID {gameId} not found.");
            }
            return slot;
        }

        public async Task<List<GameSlotWaiting>> GetGameSlotWaitlist(int gameId, int slotId)
        {
            List<GameSlotWaiting> waitlistEntries = await _db.GameSlotWaitings
                    .Where(w => w.GameSlotId == slotId && !w.IsCancelled)
                    .Include(w => w.WaitingPlayers).ThenInclude(p => p.Player)
                    .Include(w => w.RequestedBy)
                    .ToListAsync();
            return waitlistEntries;
        }

        public async Task<GameSlotWaiting> GetGameSlotWaitlistById(int waitlistId)
        {
            GameSlotWaiting entry = await _db.GameSlotWaitings
                    .FirstOrDefaultAsync(w => w.Id == waitlistId);
            if (entry == null)
            {
                throw new NotFoundCustomException($"Game Slot Waiting Entry with ID {waitlistId} not found.");
            }
            return entry;
        }

        public async Task<List<GameSlot>> GetUpcomingBookingSlotsForToday()
        {
            DateTime today = DateTime.Today;
            List<Game> games = await _db.Games.Where(g => !g.is_deleted).ToListAsync();
            List<GameSlot> upcomingSlots = new List<GameSlot>();
            foreach (var game in games)
            {
                GameSlot slots = await _db.GameSlots
                    .Where(s => s.GameId == game.Id 
                    && s.Date == today 
                    && s.EndTime > TimeOnly.FromDateTime(DateTime.Now) 
                    && s.Status == GameSlotStatus.BOOKED)
                    .Include(s => s.Game)
                    .Include(s => s.BookedBy)
                    .Include(s => s.Players)
                    .OrderBy(s => s.StartTime)
                    .FirstOrDefaultAsync();
                if (slots != null)
                {
                    upcomingSlots.Add(slots);
                }
            }
            return upcomingSlots;
        }

        public async Task<GameSlotWaiting> GetUserWaitlistEntryForSlot(int gameId, int slotId, int bookedById)
        {
            GameSlotWaiting entry = await _db.GameSlotWaitings
                   .Where(w => w.GameSlotId == slotId && w.RequestedBy.Id == bookedById)
                   .FirstOrDefaultAsync();
            if (entry == null)
            {
                throw new NotFoundCustomException($"Game Slot Waiting Entry for Slot ID {slotId} and User ID {bookedById} not found.");
            }
            return entry;
        }

        public Task<bool> IsOperationWindowOverlap(int gameId, TimeOnly operationalStartTime, TimeOnly operationalEndTime)
        {
            bool isOverlap = _db.GameOperationWindows
                            .Any(w => w.GameId == gameId && !w.is_deleted &&
                                ((operationalStartTime >= w.OperationalStartTime && operationalStartTime < w.OperationalEndTime) ||
                                 (operationalEndTime > w.OperationalStartTime && operationalEndTime <= w.OperationalEndTime) ||
                                 (operationalStartTime <= w.OperationalStartTime && operationalEndTime >= w.OperationalEndTime)));
            return Task.FromResult(isOverlap);
        }

        public Task<bool> IsUserAlreadyBookedInSlot(int gameId, int slotId, int userId)
        {
            bool isBooked = _db.GameSlotWaitings
                            .Include(w => w.WaitingPlayers)
                            .Where(w => !w.IsCancelled)
                            .Any(w => w.GameSlotId == slotId && w.WaitingPlayers.Any(p => p.PlayerId == userId));
            return Task.FromResult(isBooked);
        }

        public async Task RemovePlayerFromGameSlot(int id)
        {
            GameSlotPlayer player = await _db.GameSlotPlayers.FirstOrDefaultAsync(p => p.Id == id);
            if (player != null)
            {
                _db.GameSlotPlayers.Remove(player);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<Game> UpdateGame(Game updatedGame)
        {
            _db.Games.Update(updatedGame);
            await _db.SaveChangesAsync();
            return updatedGame;
        }

        public async Task<GameSlot> UpdateGameSlot(GameSlot slot)
        {
            _db.GameSlots.Update(slot);
            await _db.SaveChangesAsync();
            return slot;
        }

        public async Task UpdateGameSlotWaiting(GameSlotWaiting myWaitlistEntry)
        {
            _db.GameSlotWaitings.Update(myWaitlistEntry);
            await _db.SaveChangesAsync();
        }
    }
}
