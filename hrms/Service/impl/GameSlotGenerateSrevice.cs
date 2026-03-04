using hrms.Data;
using hrms.Model;
using hrms.Repository;
using hrms.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace hrms.Service.impl
{
    public class GameSlotGenerateService(
        ApplicationDbContext _db,
        IMemoryCache _cache
        ) : IGameSlotGenerateService
    {
        public async Task GenerateGameSlotsForNextXDays(DateTime today)
        {
            List<Game> games = await _db.Games
                .Where(g => !g.is_deleted)
                .Include(g => g.GameOperationWindows.Where(w => !w.is_deleted))
                .ToListAsync();

            foreach (Game game in games)
            {
                var endDate = today.Date.AddDays(game.SlotCreateForNextXDays);

                var existingSlots = await _db.GameSlots
                    .Where(s => s.GameId == game.Id && s.Date >= today.Date && s.Date < endDate)
                    .ToListAsync();

                var slotsToInsert = new List<GameSlot>();

                for (int day = 0; day < game.SlotCreateForNextXDays; day++)
                {
                    var date = today.Date.AddDays(day);

                    foreach (var window in game.GameOperationWindows)
                    {
                        var start = window.OperationalStartTime;
                        var end = window.OperationalEndTime;

                        while (start.AddMinutes(game.Duration) <= end)
                        {
                            var newStart = start;
                            var newEnd = start.AddMinutes(game.Duration);

                            bool overlap = existingSlots.Any(s =>
                                s.Date == date &&
                                s.StartTime < newEnd &&
                                s.EndTime > newStart
                            );

                            if (!overlap)
                            {
                                var slot = new GameSlot
                                {
                                    GameId = game.Id,
                                    Date = date,
                                    StartTime = newStart,
                                    EndTime = newEnd,
                                    Status = GameSlotStatus.AVAILABLE
                                };

                                slotsToInsert.Add(slot);
                                existingSlots.Add(slot);
                            }

                            start = start.AddMinutes(game.Duration);
                        }
                    }
                }

                if (slotsToInsert.Count > 0)
                {
                    await _db.GameSlots.AddRangeAsync(slotsToInsert);
                    await _db.SaveChangesAsync();
                }

                IncrementCacheVersion(CacheVersionKey.ForGameSlots(game.Id));
            }
            // List<Game> games = await _db.Games
            //                 .Where(g => !g.is_deleted)
            //                 .Include(s => s.GameOperationWindows.Where(w => !w.is_deleted))
            //                 .ToListAsync();
            // foreach (Game game in games)
            // {
            //     for (int day = 0; day < game.SlotCreateForNextXDays; day++)
            //     {
            //         var date = today.Date.AddDays(day);

            //         bool anySlotsExist = await _db.GameSlots
            //             .AnyAsync(s => s.GameId == game.Id && s.Date == date);
            //         //if (anySlotsExist) // this means slots are already generated for this date, so skip to next date
            //         //    continue;

            //         foreach(var window in game.GameOperationWindows)
            //         {
            //             var start = window.OperationalStartTime;
            //             var end = window.OperationalEndTime;
            //             while (start.AddMinutes(game.Duration) <= end)
            //             {
            //                 var newStart = start;
            //                 var newEnd = start.AddMinutes(game.Duration);
            //                 bool exists = await _db.GameSlots
            //                             .AnyAsync(
            //                                 s => s.GameId == game.Id && 
            //                                 s.Date == date && 
            //                                 s.StartTime < newEnd && 
            //                                 s.EndTime > newStart);
            //                 if (!exists)
            //                 {
            //                     GameSlot slot = new GameSlot
            //                     {
            //                         GameId = game.Id,
            //                         Date = date,
            //                         StartTime = start,
            //                         EndTime = start.AddMinutes(game.Duration),
            //                         Status = GameSlotStatus.AVAILABLE,
            //                     };
            //                     await _db.GameSlots.AddAsync(slot);
            //                 }
            //                 start = start.AddMinutes(game.Duration);
            //             }
            //         }
            //     }
            //     await _db.SaveChangesAsync();
            //     IncrementCacheVersion(CacheVersionKey.ForGameSlots(game.Id));
            // }
        }

        private void IncrementCacheVersion(string versionKey)
        {
            var current = _cache.Get<int>(versionKey);
            _cache.Set(versionKey, current + 1);
        }
    }
}