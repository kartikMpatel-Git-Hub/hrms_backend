using hrms.Service;
using Quartz;
namespace hrms.Utility
{
    public class WeeklyJob(
        IGameSlotGenerateService _gameSlotGenerateService
        ) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await _gameSlotGenerateService.GenerateGameSlotsForNextXDays(DateTime.Now);
        }
    }
}
