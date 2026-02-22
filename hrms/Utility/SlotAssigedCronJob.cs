using hrms.Service;

namespace hrms.Utility
{
    public class SlotAssignedCronJob(IServiceScopeFactory _scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var allocator = scope.ServiceProvider.GetRequiredService<ISlotBookingService>();

                await allocator.ProcessSlotsAsync();

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}