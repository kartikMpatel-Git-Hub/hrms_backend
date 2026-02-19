using System.Security.Claims;
using hrms.Dto.Request.BookingSlot;
using hrms.Dto.Response.BookingSlot;
using hrms.Dto.Response.Game.offere;
using hrms.Model;

namespace hrms.Service
{
    public interface ISlotBookingService
    {
        Task AcceptOffer(int offerId, BookSlotRequestDto dto);

        // Task<List<BookingSlot>> GetAvailableSlots(int gameId, DateTime date);
        Task<BookingSlotResponseDto> BookSlot(int slotId, int userId,BookSlotRequestDto dto);
        Task<List<SlotOffereResponseDto>> GetActiveOfferes(int currentUser,int gameId);
        Task RejectOffer(int offerId);
        // Task CancelBooking(int slotId, int userId);
        // Task<List<BookingSlot>> GetUserBookings(int userId);
        // Task OfferSlot(int slotId, int fromUserId, int toUserId);
        // Task RespondToOffer(int offerId, bool accept);
    }
}