using hrms.Dto.Request.BookingSlot;
using hrms.Model;
using hrms.Repository;
using Quartz;
namespace hrms.Utility
{
    public class WeeklyJob(
        IGameRepository _gameRepository,
        IGameBookingRepository _gameBookingRepository
        ) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            List<Game> games = await _gameRepository.GetAllGames();
            foreach (Game game in games)
            {
                List<GameSlot> gameSlots = await _gameRepository.GetGameSlots(game.Id);
                foreach (GameSlot slot in gameSlots)
                {
                    DateTime current = DateTime.Now;
                        var isExists = await _gameBookingRepository.existsSlot(game.Id, slot.StartTime, slot.EndTime, current);

                        if (isExists)
                        {
                            continue;
                        }

                        BookingSlot newSlot = new BookingSlot()
                        {
                            Game = game,
                            GameId = game.Id,
                            StartTime = slot.StartTime,
                            EndTime = slot.EndTime,
                            Date = current,
                            Status = GameSlotStatus.AVAILABLE
                        };
                        await _gameBookingRepository.CreateSlot(newSlot);
                }
            }
        }
    }
}
