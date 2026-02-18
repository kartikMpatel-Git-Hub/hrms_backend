using hrms.CustomException;
using hrms.Data;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Repository.impl
{
    public class GameBookingRepository(ApplicationDbContext _db) : IGameBookingRepository
    {
        public async Task<BookingSlot> BookSlot(BookingSlot createSlot)
        {
            BookingSlot slot = _db.WeeklyGameSlots.Update(createSlot).Entity;
            await _db.SaveChangesAsync();
            return slot;
        }

        public async Task<BookingPlayer> AddPlayers(BookingPlayer bookingPlayer)
        {
            var addedEntity = await _db.BookingPlayers.AddAsync(bookingPlayer);
            await _db.SaveChangesAsync();
            return addedEntity.Entity;
        }

        public async Task<BookingPlayer> GetBookingPlayerById(int bookingId)
        {
            BookingPlayer booking = await _db.BookingPlayers.
                    Where(b => b.Id == bookingId).FirstOrDefaultAsync();
            if (booking == null)
                throw new NotFoundCustomException($"Booking with Id : {bookingId} Not Found !");
            return booking;
        }

        public async Task RemovePlayer(int bookingId)
        {
            BookingPlayer bookingPlayer = await GetBookingPlayerById(bookingId);
            bookingPlayer.is_deleted = true;
            _db.BookingPlayers.Update(bookingPlayer);
            await _db.SaveChangesAsync();
        }

        public async Task<BookingSlot> ChangeStatus(BookingSlot updatedSlot)
        {
            BookingSlot slot = _db.WeeklyGameSlots.Update(updatedSlot).Entity;
            await _db.SaveChangesAsync();
            return slot;
        }

        public async Task<BookingSlot> CreateSlot(BookingSlot slot)
        {
            var savedEntity = await _db.WeeklyGameSlots.AddAsync(slot);
            await _db.SaveChangesAsync();
            Console.WriteLine("Slot Created !");
            return savedEntity.Entity;
        }

        public async Task<BookingSlot> GetSlot(int slotId)
        {
            BookingSlot slot = await _db.WeeklyGameSlots
                .Where(
                    (wgs) => 
                        wgs.Id == slotId && 
                        wgs.is_deleted == false
                    )
                .Include((b) => b.Players.Where((p) => p.is_deleted == false))
                .Include(s => s.Game)
                .FirstOrDefaultAsync();
            return slot == null ? throw new NotFoundCustomException($"BookingSlot with Id : {slotId} Not Found !") : slot;
        }

        public async Task<List<BookingSlot>> GetSlots(int GameId,DateTime from, DateTime to)
        {
            List<BookingSlot> slot = await _db.WeeklyGameSlots
                .Where(
                    (wgs) =>
                        wgs.GameId == GameId &&
                        wgs.is_deleted == false &&
                        wgs.Date.Date >= from.Date && wgs.Date.Date <= to.Date
                    )
                .ToListAsync();
            return slot;
        }

        public async Task<bool> existsSlot(int id, TimeOnly startTime, TimeOnly endTime, DateTime now)
        {
            bool exists = await _db.WeeklyGameSlots.Where(
                   s => s.GameId == id && 
                   s.StartTime == startTime && 
                   s.EndTime == endTime &&
                   s.Date.Date == now.Date
                ).AnyAsync();
            return exists;
        }
    }
}
