using hrms.CustomException;
using hrms.Data;
using hrms.Dto.Response.Other;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Repository.impl
{
    public class GameRepository(ApplicationDbContext _db) : IGameRepository
    {

        public async Task<Game> CreateGame(Game newGame)
        {
            var SavedEntity = await _db.Games.AddAsync(newGame);
            await _db.SaveChangesAsync();
            return SavedEntity.Entity;
        }

        public async Task<GameSlot> CreateGameSlot(GameSlot newSlot)
        {
            var savedGameSlot = await _db.GameSlots.AddAsync(newSlot);
            await _db.SaveChangesAsync();
            return savedGameSlot.Entity;
        }

        public async Task<PagedReponseOffSet<Game>> GetAllGames(int pageNumber, int pageSize)
        {
            int totalData = await _db.Games.Where(g => g.is_deleted == false).CountAsync();
            List<Game> games = await _db.Games
                                .Where(g => g.is_deleted == false)
                                .Skip((pageNumber-1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            PagedReponseOffSet<Game> reponse = new PagedReponseOffSet<Game>(games, pageNumber, pageSize, totalData);
            return reponse;
        }

        public async Task<List<Game>> GetAllGames()
        {
            List<Game> games = await _db.Games
                                .Where(g => g.is_deleted == false)
                                .ToListAsync();
            return games;
        }

        public async Task<Game> GetGameById(int gameId)
        {
            Game game = await _db.Games
                .Where((g) => g.Id == gameId)
                .Include(g => g.Slots.Where(s => s.is_deleted == false).OrderBy(s => s.StartTime))
                .FirstOrDefaultAsync();
            if (game == null)
                throw new NotFoundCustomException($"Game with id : {gameId} Not Found !");
            return game;
        }

        public async Task<GameSlot> GetGameSlotById(int gameSlotId)
        {
            GameSlot gameSlot = await _db.GameSlots.Where((gs) => gs.Id == gameSlotId).FirstOrDefaultAsync();
            if (gameSlot == null)
                throw new NotFoundCustomException($"Game slot with id : {gameSlotId} Not Found !");
            return gameSlot;
        }

        public async Task<List<GameSlot>> GetGameSlots(int gameId)
        {
            List<GameSlot> slots = await _db.GameSlots.Where(gs => gs.GameId ==  gameId).ToListAsync();
            return slots;
        }
        public async Task<bool> isSlotExist(TimeOnly startTime, TimeOnly endTime)
        {

            return await _db.GameSlots.AnyAsync(gs =>
                    !gs.is_deleted &&
                    startTime < gs.EndTime &&  
                    endTime > gs.StartTime      
                );

        }

        public async Task RemoveGame(Game game)
        {
            game.is_deleted = true;
            _db.Games.Update(game);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveGameSlot(GameSlot gameSlot)
        {
            gameSlot.is_deleted = true;
            _db.GameSlots.Update(gameSlot);
            await _db.SaveChangesAsync();
        }

        public async Task<Game> UpdateGame(Game updatedGame)
        {
            var entityEntry = _db.Games.Update(updatedGame);
            await _db.SaveChangesAsync();
            return entityEntry.Entity;
        }

        public async Task<GameSlot> UpdateGameSlot(GameSlot updatedGameSlot)
        {
            var entityEntry = _db.GameSlots.Update(updatedGameSlot);
            await _db.SaveChangesAsync();
            return entityEntry.Entity;
        }
    }
}
