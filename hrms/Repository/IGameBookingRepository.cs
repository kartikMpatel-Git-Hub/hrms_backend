using hrms.Model;

namespace hrms.Repository
{
    public interface IGameBookingRepository
    {

        Task<BookingSlot> CreateSlot(BookingSlot slot);
        Task<BookingSlot> BookSlot(BookingSlot createSlot);
        Task<BookingPlayer> AddPlayers(BookingPlayer bookingPlayer);
        Task RemovePlayer(int bookingId);
        Task<BookingSlot> GetSlot(int slotId);
        Task<List<BookingSlot>> GetSlots(int GameId,DateTime from, DateTime to);
        Task<BookingSlot> ChangeStatus(BookingSlot updatedSlot);
        Task<bool> existsSlot(int id, TimeOnly startTime, TimeOnly endTime, DateTime now);
        Task CreateSlotOffer(SlotOffere offer);
        Task<SlotOffere> GetSlotOffer(int offerId);
        Task UpdateSlotOffer(SlotOffere offer);
        Task ExpriredOffrers(int bookingSlotId);
    }
}
