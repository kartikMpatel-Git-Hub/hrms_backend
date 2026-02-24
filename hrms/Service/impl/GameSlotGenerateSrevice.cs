using hrms.Data;
using hrms.Model;
using hrms.Repository;
using Microsoft.EntityFrameworkCore;

namespace hrms.Service.impl
{
    public class GameSlotGenerateService(ApplicationDbContext _db) : IGameSlotGenerateService
    {
        public async Task GenerateGameSlotsForNextXDays(DateTime today)
        {
            List<Game> games = await _db.Games
                            .Where(g => !g.is_deleted)
                            .Include(s => s.GameOperationWindows.Where(w => !w.is_deleted))
                            .ToListAsync();
            foreach (Game game in games)
            {
                for (int day = 0; day < game.SlotCreateForNextXDays; day++)
                {
                    var date = today.Date.AddDays(day);
                    foreach(var window in game.GameOperationWindows)
                    {
                        var start = window.OperationalStartTime;
                        var end = window.OperationalEndTime;
                        while (start.AddMinutes(game.Duration) <= end)
                        {
                            bool exists = await _db.GameSlots
                                        .AnyAsync(s => s.GameId == game.Id && s.Date == date && s.StartTime == start);
                            //exists = await _db.GameSlots
                            //            .AnyAsync(s => s.GameId == game.Id
                            //            && s.Date == date
                            //            && start > s.StartTime && start <= s.EndTime);
                            if (!exists)
                            {
                                GameSlot slot = new GameSlot
                                {
                                    GameId = game.Id,
                                    Date = date,
                                    StartTime = start,
                                    EndTime = start.AddMinutes(game.Duration),
                                    Status = GameSlotStatus.AVAILABLE,
                                };
                                await _db.GameSlots.AddAsync(slot);
                            }
                            start = start.AddMinutes(game.Duration);
                        }
                    }
                }
                await _db.SaveChangesAsync();
            }
        }
    }
}