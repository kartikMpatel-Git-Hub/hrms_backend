using hrms.Data;
using hrms.Model;
using hrms.Repository;
using Microsoft.EntityFrameworkCore;

namespace hrms.Service.impl
{
    public class SlotBookingService(
        // IGameBookingRepository _repository,
        IGameRepository _repository,
        IUserGameRepository _userGameRpository,
        ApplicationDbContext _db,
        IEmailService emailService
        ) : ISlotBookingService
    {
        // public async Task AcceptOffer(int offerId, BookSlotRequestDto dto)
        // {
        //     SlotOffere offer = await _repository.GetSlotOffer(offerId);
        //     if (offer == null || offer.Status != SlotOfferStatus.Pending)
        //     {
        //         throw new InvalidOperationCustomException($"Invalid Offer !");
        //     }
        //     if (offer.ExpiredAt < DateTime.Now)
        //     {
        //         offer.Status = SlotOfferStatus.Expired;
        //         await _repository.UpdateSlotOffer(offer);
        //         throw new InvalidOperationCustomException($"Offer is Expired !");
        //     }
        //     BookingSlot slot = offer.Slot;

        //     if (slot == null || slot.Status != GameSlotStatus.WAITING)
        //     {
        //         throw new InvalidOperationCustomException($"Slot is not Available !");
        //     }

        //     foreach (var playerId in dto.Players)
        //     {
        //         if (!await _userGameRpository.IsUserInterestedInGame(playerId, slot.GameId))
        //         {
        //             throw new InvalidOperationCustomException($"Player with Id : {playerId} is not Interested in Game with Id : {slot.GameId} !");
        //         }
        //     }

        //     slot.BookedBy = offer.OffereTo;
        //     slot.Booked = await _userRepository.GetByIdAsync(offer.OffereTo);
        //     slot.Status = GameSlotStatus.BOOKED;
        //     await _repository.ChangeStatus(slot);
        //     foreach (var playerId in dto.Players)
        //     {
        //         var bookingPlayer = new BookingPlayer()
        //         {
        //             PlayerId = playerId,
        //             SlotId = slot.Id
        //         };
        //         await _repository.AddPlayers(bookingPlayer);
        //     }
        //     offer.Status = SlotOfferStatus.Accepted;
        //     await _repository.ExpriredOffrers(offer.BookingSlotId);
        //     await _repository.UpdateSlotOffer(offer);
        // }
        // public async Task<BookingSlotResponseDto> BookSlot(int slotId, int userId, BookSlotRequestDto dto)
        // {
        //     using var tx = _db.Database.BeginTransactionAsync();
        //     BookingSlot slot = await _repository.GetSlot(slotId);
        //     if (slot.Status != GameSlotStatus.AVAILABLE)
        //     {
        //         throw new InvalidOperationCustomException($"Slot with Id : {slotId} is not Available !");
        //     }
        //     if (!await _userGameRpository.IsUserInterestedInGame(userId, slot.GameId))
        //     {
        //         throw new InvalidOperationCustomException($"You Have not mark this game as Interested !");
        //     }
        //     foreach (var playerId in dto.Players)
        //     {
        //         if (!await _userGameRpository.IsUserInterestedInGame(playerId, slot.GameId))
        //         {
        //             throw new InvalidOperationCustomException($"Player with Id : {playerId} is not Interested in Game with Id : {slot.GameId} !");
        //         }
        //     }
        //     UserGameState userGameState = await _userGameRpository.GetUserGameState(userId, slot.GameId);
        //     List<int> hightPriority = await _userGameRpository.GetUserGameStates(slot.GameId, userGameState.GamePlayed);
        //     if (hightPriority.Count == 0)
        //     {
        //         var user = await _userRepository.GetByIdAsync(userId);
        //         slot.BookedBy = userId;
        //         slot.Booked = user;
        //         slot.Status = GameSlotStatus.BOOKED;
        //         slot.RequestBy = userId;
        //         slot.Requester = user;
        //         BookingSlot response = await _repository.BookSlot(slot);
        //         List<string> mailTo = new List<string>();
        //         foreach (var playerId in dto.Players)
        //         {
        //             var bookingPlayer = new BookingPlayer()
        //             {
        //                 PlayerId = playerId,
        //                 SlotId = response.Id
        //             };
        //             await _repository.AddPlayers(bookingPlayer);
        //             User p = await _db.Users.Where(u => u.Id == playerId && u.is_deleted == false).FirstOrDefaultAsync();
        //             if (p != null)
        //                 mailTo.Add(p.Email);
        //         }
        //         userGameState.GamePlayed += 1;
        //         await _userGameRpository.UpdateuserState(userGameState);
        //         await _db.SaveChangesAsync();
        //         await tx.Result.CommitAsync();
        //         System.Console.WriteLine("saved");

        //         emailService.SendEmailAsync(user.Email, "Slot Booked", $"Your booking for slot {slot.Id} is confirmed.").Wait();
        //         foreach (var email in mailTo)
        //         {
        //             emailService.SendEmailAsync(email, "Slot Booked", $"Slot {slot.Id} is booked by {user.Email} user.").Wait();
        //         }
        //         return _mapper.Map<BookingSlotResponseDto>(response);
        //     }
        //     System.Console.WriteLine("Here3");
        //     slot.Status = GameSlotStatus.WAITING;
        //     slot.RequestBy = userId;
        //     slot.Requester = await _userRepository.GetByIdAsync(userId);
        //     foreach (var playerId in dto.Players)
        //     {
        //         var requestedPlayer = new RequestedPlayer()
        //         {
        //             PlayerId = playerId,
        //             SlotId = slot.Id
        //         };
        //         await _repository.AddPlayerToRequest(requestedPlayer);
        //     }
        //     int idx = 1;
        //     // how to prvent sending offere to those who are already in request list
        //     foreach (var playerId in hightPriority.Where(hp => !dto.Players.Contains(hp) && hp != userId).ToList())
        //     {
        //         var offer = new SlotOffere()
        //         {
        //             BookingSlotId = slot.Id,
        //             OffereTo = playerId,
        //             PriorityOrder = idx++,
        //             Status = SlotOfferStatus.Pending
        //         };
        //         await _repository.CreateSlotOffer(offer);
        //     }

        //     BookingSlot bookingSlot = await _repository.BookSlot(slot);
        //     await _db.SaveChangesAsync();
        //     await tx.Result.CommitAsync();

        //     return _mapper.Map<BookingSlotResponseDto>(bookingSlot);
        // }

        // public async Task<List<SlotOffereResponseDto>> GetActiveOfferes(int currentUser, int gameId)
        // {
        //     List<SlotOffere> slotOfferes = await _db.SlotOffers
        //                         .Where(s => s.Status == SlotOfferStatus.InProcess
        //                         && s.OffereTo == currentUser
        //                         && s.Slot.GameId == gameId
        //                         )
        //                         .Include(s => s.Slot)
        //                         .ToListAsync();
        //     return _mapper.Map<List<SlotOffereResponseDto>>(slotOfferes);
        // }

        public async Task ProcessSlotsAsync()
        {
            List<GameSlot> waitingSlots = await _db.GameSlots
                                .Where(s => s.Status == GameSlotStatus.AVAILABLE || s.Status == GameSlotStatus.WAITING)
                                .Include(s => s.Game)
                                .ToListAsync();
            foreach (var slot in waitingSlots)
            {
                if (slot != null)
                    await AllocateSlot(slot);
            }
        }

        private async Task AllocateSlot(GameSlot slot)
        {
            var slotStartDateTime =
            slot.Date.Add(slot.StartTime.ToTimeSpan());
            var slotEndDateTime =
            slot.Date.Add(slot.EndTime.ToTimeSpan());
            if (DateTime.Now >= slotEndDateTime)
            {
                slot.Status = GameSlotStatus.COMPLETED;
                await _repository.UpdateGameSlot(slot);
                return;
            }
            if (DateTime.Now <
                slotStartDateTime.AddMinutes(-slot.Game.SlotAssignedBeforeMinutes))
            {
                return;
            }

            //var waiting = await _db.GameSlotWaitings
            //.Where(w => w.GameSlotId == slot.Id && w.IsCancelled == false)
            //.Select(w => new
            //{
            //    Waiting = w,
            //    State = _db.UserGameStates
            //        .FirstOrDefault(s =>
            //            s.UserId == w.RequestedById &&
            //            s.GameId == slot.GameId)
            //})
            //.OrderBy(x =>
            //    x.State == null || x.State.GamePlayed == 0 ? 0 : 1)
            //.ThenBy(x => x.State != null ? x.State.GamePlayed : 0)
            //.ThenBy(x => x.State != null ? x.State.LastPlayedAt : DateTime.MinValue)
            //.ThenBy(x => x.Waiting.RequestedAt)
            //.ThenBy(x => x.Waiting.Id)
            //.FirstOrDefaultAsync();

            var waitingGroup = await
                (
                    from w in _db.GameSlotWaitings
                    where w.GameSlotId == slot.Id && !w.IsCancelled
                    select new
                    {
                        Waiting = w,
                        UserId = w.RequestedById
                    }
                )
                .Union
                (
                    from w in _db.GameSlotWaitings
                    join p in _db.GameSlotWaitingPlayers
                        on w.Id equals p.GameSlotWaitingId
                    where w.GameSlotId == slot.Id && !w.IsCancelled
                    select new
                    {
                        Waiting = w,
                        UserId = p.PlayerId
                    }
                )
                .GroupJoin(
                    _db.UserGameStates.Where(s => s.GameId == slot.GameId),
                    x => x.UserId,
                    s => s.UserId,
                    (x, gs) => new { x.Waiting, GameStates = gs }
                )
                .SelectMany(
                    x => x.GameStates.DefaultIfEmpty(),
                    (x, s) => new
                    {
                        Waiting = x.Waiting,
                        GamePlayed = s != null ? s.GamePlayed : 0,
                        LastPlayedAt = (DateTime?)s.LastPlayedAt
                    }
                )
                .GroupBy(x => x.Waiting)
                .Select(g => new
                {
                    Waiting = g.Key,
                    AverageGamePlayed = g.Average(x => x.GamePlayed),
                    LastPlayedAt = g.Max(x => x.LastPlayedAt),
                    HasHistory = g.Any(x => x.GamePlayed > 0)
                })
                .OrderBy(x => x.HasHistory ? 1 : 0)
                .ThenBy(x => x.AverageGamePlayed)
                .ThenBy(x => x.LastPlayedAt ?? DateTime.MinValue)
                .ThenBy(x => x.Waiting.RequestedAt)
                .ThenBy(x => x.Waiting.Id)
                .FirstOrDefaultAsync();

            if (waitingGroup == null)
            {
                return;
            }
            var players = await _db.GameSlotWaitingPlayers
                        .Where(p => p.GameSlotWaitingId == waitingGroup.Waiting.Id)
                        .ToArrayAsync();

            using var tx = await _db.Database.BeginTransactionAsync();

            foreach (var player in players)
            {
                _db.GameSlotPlayers.Add(new GameSlotPlayer
                {
                    SlotId = slot.Id,
                    PlayerId = player.PlayerId
                });
                await UpdateUserGameState(player.PlayerId, slot.GameId);
            }

            await UpdateUserGameState(waitingGroup.Waiting.RequestedById, slot.GameId);

            slot.BookedById = waitingGroup.Waiting.RequestedById;
            slot.Status = GameSlotStatus.BOOKED;
            slot.BookedAt = DateTime.Now;
            _db.GameSlots.Update(slot);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            await SendNotificationAndMail(slot, players);
        }

        private async Task SendNotificationAndMail(GameSlot slot, GameSlotWaitingPlayer[] players)
        {
            var u = await _db.Users
                    .Where(u => u.Id == slot.BookedById).FirstOrDefaultAsync();
            if (u == null)
                return;
            var notification = new Notification
            {
                NotifiedTo = u.Id,
                Title = "Slot Booked",
                Description = $"Your booking for slot {slot.Id} is confirmed.",
                NotificationDate = DateTime.Now
            };
            await _db.Notifications.AddAsync(notification);
            await _db.SaveChangesAsync();
            await emailService.SendEmailAsync(
                u.Email,
                "Slot Booked",
                $"Your booking for slot {slot.Id} is confirmed.");

            foreach (var player in players)
            {
                var user = await _db.Users
                        .Where(u => u.Id == player.PlayerId && u.is_deleted == false)
                        .FirstOrDefaultAsync();

                if (user != null)
                {
                    await emailService.SendEmailAsync(user.Email, "Slot Booked", $"Slot {slot.Id} is booked by {slot.BookedBy.Email} user.");
                    var n = new Notification
                    {
                        NotifiedTo = user.Id,
                        Title = "Slot Booked",
                        Description = $"Your booking for slot {slot.Id} is confirmed.",
                        NotificationDate = DateTime.Now
                    };
                    await _db.Notifications.AddAsync(n);
                }
            }
        }

        private async Task UpdateUserGameState(int playerId, int gameId)
        {
            var userGameState = _db.UserGameStates.FirstOrDefault(s => s.UserId == playerId && s.GameId == gameId);
            if (userGameState == null)
            {
                userGameState = new UserGameState
                {
                    UserId = playerId,
                    GameId = gameId,
                    GamePlayed = 1,
                    LastPlayedAt = DateTime.Now
                };
                await _db.UserGameStates.AddAsync(userGameState);
            }
            else
            {
                userGameState.GamePlayed++;
                userGameState.LastPlayedAt = DateTime.Now;
                _db.UserGameStates.Update(userGameState);
            }
        }

        // // public async Task RejectOffer(int offerId)
        // {
        //     SlotOffere offer = await _repository.GetSlotOffer(offerId);
        //     if (offer.Status != SlotOfferStatus.Pending)
        //     {
        //         throw new InvalidOperationCustomException($"Invalid Offer !");
        //     }
        //     offer.Status = SlotOfferStatus.Expired;
        //     await _repository.UpdateSlotOffer(offer);
        //     BookingSlot slot = await _db.BookingSlots.Where(s => s.Id == offer.BookingSlotId).FirstOrDefaultAsync();
        //     if (slot == null)
        //         return;
        //     System.Console.WriteLine("Sending Offere to next player");
        //     var nextOrdere = slot.CurrentPriorityOrder + 1;
        //     var nextOfere = slot.Offers.
        //             FirstOrDefault(o => o.PriorityOrder == nextOrdere && o.Status == SlotOfferStatus.Pending);
        //     if (nextOfere == null)
        //         return;
        //     nextOfere.Status = SlotOfferStatus.Pending;
        //     nextOfere.CreatedAt = DateTime.UtcNow;
        //     slot.CurrentPriorityOrder = nextOrdere;
        //     User user = await _db.Users.Where(u => u.Id == nextOfere.OffereTo).FirstOrDefaultAsync();
        //     if (user != null)
        //         await emailService.SendEmailAsync(user.Email, "Slot Offer", $"Your offer for slot {slot.Id} is now active. Please respond within 5 minutes.");
        //     System.Console.WriteLine("Offere Mail Sended !");
        // }
    }
}