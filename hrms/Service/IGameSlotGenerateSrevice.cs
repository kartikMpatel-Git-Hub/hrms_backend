namespace hrms.Service
{
    public interface IGameSlotGenerateService
    {
        Task GenerateGameSlotsForNextXDays(DateTime date);
    }
}