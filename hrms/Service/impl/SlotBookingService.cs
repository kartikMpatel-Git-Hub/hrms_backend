using System.Security.Claims;
using AutoMapper;
using hrms.CustomException;
using hrms.Data;
using hrms.Dto.Request.BookingSlot;
using hrms.Dto.Response.BookingSlot;
using hrms.Dto.Response.Game.offere;
using hrms.Model;
using hrms.Repository;
using Microsoft.EntityFrameworkCore;

namespace hrms.Service.impl
{
    public class SlotBookingService(
        IGameBookingRepository _repository,
        ApplicationDbContext _db,
        IUserRepository _userRepository,
        IMapper _mapper,
        IUserGameRepository _userGameRpository,
        IEmailService emailService
        ) : ISlotBookingService
    {
        public async Task AcceptOffer(int offerId, BookSlotRequestDto dto)
        {
            SlotOffere offer = await _repository.GetSlotOffer(offerId);
            if (offer == null || offer.Status != SlotOfferStatus.Pending)
            {
                throw new InvalidOperationCustomException($"Invalid Offer !");
            }
            if (offer.ExpiredAt < DateTime.Now)
            {
                offer.Status = SlotOfferStatus.Expired;
                await _repository.UpdateSlotOffer(offer);
                throw new InvalidOperationCustomException($"Offer is Expired !");
            }
            BookingSlot slot = offer.Slot;

            if (slot == null || slot.Status != GameSlotStatus.WAITING)
            {
                throw new InvalidOperationCustomException($"Slot is not Available !");
            }

            foreach (var playerId in dto.Players)
            {
                if (!await _userGameRpository.IsUserInterestedInGame(playerId, slot.GameId))
                {
                    throw new InvalidOperationCustomException($"Player with Id : {playerId} is not Interested in Game with Id : {slot.GameId} !");
                }
            }

            slot.BookedBy = offer.OffereTo;
            slot.Booked = await _userRepository.GetByIdAsync(offer.OffereTo);
            slot.Status = GameSlotStatus.BOOKED;
            await _repository.ChangeStatus(slot);
            foreach (var playerId in dto.Players)
            {
                var bookingPlayer = new BookingPlayer()
                {
                    PlayerId = playerId,
                    SlotId = slot.Id
                };
                await _repository.AddPlayers(bookingPlayer);
            }
            offer.Status = SlotOfferStatus.Accepted;
            await _repository.ExpriredOffrers(offer.BookingSlotId);
            await _repository.UpdateSlotOffer(offer);
        }
        public async Task<BookingSlotResponseDto> BookSlot(int slotId, int userId, BookSlotRequestDto dto)
        {
            using var tx = _db.Database.BeginTransactionAsync();
            BookingSlot slot = await _repository.GetSlot(slotId);
            if (slot.Status != GameSlotStatus.AVAILABLE)
            {
                throw new InvalidOperationCustomException($"Slot with Id : {slotId} is not Available !");
            }
            if (!await _userGameRpository.IsUserInterestedInGame(userId, slot.GameId))
            {
                throw new InvalidOperationCustomException($"User with Id : {userId} is not Interested in Game with Id : {slot.GameId} !");
            }
            foreach (var playerId in dto.Players)
            {
                if (!await _userGameRpository.IsUserInterestedInGame(playerId, slot.GameId))
                {
                    throw new InvalidOperationCustomException($"Player with Id : {playerId} is not Interested in Game with Id : {slot.GameId} !");
                }
            }
            UserGameState userGameState = await _userGameRpository.GetUserGameState(userId, slot.GameId);
            // System.Console.WriteLine(userGameState.GamePlayed);
            List<int> hightPriority = await _userGameRpository.GetUserGameStates(slot.GameId, userGameState.GamePlayed);
            // System.Console.WriteLine(hightPriority.Count);
            // return null;
            if (hightPriority.Count == 0)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                slot.BookedBy = userId;
                slot.Booked = user;
                slot.Status = GameSlotStatus.BOOKED;
                slot.RequestBy = userId;
                slot.Requester = user;
                BookingSlot response = await _repository.BookSlot(slot);
                List<string> mailTo = new List<string>();
                foreach (var playerId in dto.Players)
                {
                    var bookingPlayer = new BookingPlayer()
                    {
                        PlayerId = playerId,
                        SlotId = response.Id
                    };
                    await _repository.AddPlayers(bookingPlayer);
                    User p = await _db.Users.Where(u => u.Id == playerId && u.is_deleted == false).FirstOrDefaultAsync();
                    if (p != null)
                        mailTo.Add(p.Email);
                }
                userGameState.GamePlayed += 1;
                await _userGameRpository.UpdateuserState(userGameState);
                await _db.SaveChangesAsync();
                await tx.Result.CommitAsync();
                System.Console.WriteLine("saved");
                
                emailService.SendEmailAsync(user.Email, "Slot Booked", $"Your booking for slot {slot.Id} is confirmed.").Wait();
                foreach (var email in mailTo)                {
                    emailService.SendEmailAsync(email, "Slot Booked", $"Slot {slot.Id} is booked by {user.Email} user.").Wait();
                }
                return _mapper.Map<BookingSlotResponseDto>(response);
            }
            System.Console.WriteLine("Here3");
            slot.Status = GameSlotStatus.WAITING;
            slot.RequestBy = userId;
            slot.Requester = await _userRepository.GetByIdAsync(userId);
            foreach (var playerId in dto.Players)
            {
                var requestedPlayer = new RequestedPlayer()
                {
                    PlayerId = playerId,
                    SlotId = slot.Id
                };
                slot.RequestedPlayers.Add(requestedPlayer);
            }
            int idx = 1;
            foreach (var playerId in hightPriority)
            {
                var offer = new SlotOffere()
                {
                    BookingSlotId = slot.Id,
                    OffereTo = playerId,
                    PriorityOrder = idx++,
                    Status = SlotOfferStatus.Pending
                };
                await _repository.CreateSlotOffer(offer);
            }

            BookingSlot bookingSlot = await _repository.BookSlot(slot);
            await _db.SaveChangesAsync();
            await tx.Result.CommitAsync();

            return _mapper.Map<BookingSlotResponseDto>(bookingSlot);
        }

        public async Task<List<SlotOffereResponseDto>> GetActiveOfferes(int currentUser, int gameId)
        {
            List<SlotOffere> slotOfferes = await _db.SlotOffers
                                .Where(s => s.Status == SlotOfferStatus.InProcess
                                && s.OffereTo == currentUser
                                && s.Slot.GameId == gameId
                                )
                                .Include(s => s.Slot)
                                .ToListAsync();
            return _mapper.Map<List<SlotOffereResponseDto>>(slotOfferes);
        }

        public async Task RejectOffer(int offerId)
        {
            SlotOffere offer = await _repository.GetSlotOffer(offerId);
            if (offer.Status != SlotOfferStatus.Pending)
            {
                throw new InvalidOperationCustomException($"Invalid Offer !");
            }
            offer.Status = SlotOfferStatus.Expired;
            await _repository.UpdateSlotOffer(offer);
            BookingSlot slot = await _db.BookingSlots.Where(s => s.Id == offer.BookingSlotId).FirstOrDefaultAsync();
            if (slot == null)
                return;
            System.Console.WriteLine("Sending Offere to next player");
            var nextOrdere = slot.CurrentPriorityOrder + 1;
            var nextOfere = slot.Offers.
                    FirstOrDefault(o => o.PriorityOrder == nextOrdere && o.Status == SlotOfferStatus.Pending);
            if (nextOfere == null)
                return;
            nextOfere.Status = SlotOfferStatus.Pending;
            nextOfere.CreatedAt = DateTime.UtcNow;
            slot.CurrentPriorityOrder = nextOrdere;
            User user = await _db.Users.Where(u => u.Id == nextOfere.OffereTo).FirstOrDefaultAsync();
            if (user != null)
                await emailService.SendEmailAsync(user.Email, "Slot Offer", $"Your offer for slot {slot.Id} is now active. Please respond within 5 minutes.");
            System.Console.WriteLine("Offere Mail Sended !");
        }
    }
}