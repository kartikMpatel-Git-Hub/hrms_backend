using hrms.Dto.Request.BookingSlot;
using hrms.Dto.Response.BookingSlot;
using hrms.Dto.Response.Game;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.User;

namespace hrms.Service
{
    public interface IGameSlotService
    {
        Task<BookingSlotResponseDto> CreateSlot(BookingSlotCreateDto dto);
        Task<BookingSlotResponseDto> DeleteSlot(int slotId);
        Task RemovePlayer(int bookingId);
        Task<BookingSlotWithPlayerResponseDto> GetSlot(int slotId);
        Task<List<BookingSlotResponseDto>> GetBookingSlots(int gameId, DateTime from, DateTime to);
        Task<BookingSlotResponseDto> BookSlot(int bookingSlotId, int currentUserId, BookSlotRequestDto dto);
        Task<PagedReponseDto<UserResponseDto>> GetAvailablePlayers(int gameId, int currentUserId, string? key, int pageSize, int pageNumber);
    }
}
