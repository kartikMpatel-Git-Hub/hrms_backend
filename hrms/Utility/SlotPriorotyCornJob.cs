using hrms.CustomException;
using hrms.Data;
using hrms.Model;
using hrms.Service;
using Microsoft.EntityFrameworkCore;
using Quartz.Impl.AdoJobStore;

namespace hrms.Utility
{
    public class SlotPriorotyCornJob(IServiceScopeFactory _serviceScopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessSlots();
                await Task.Delay(TimeSpan.FromMinutes(4), stoppingToken);
            }
        }

        private async Task ProcessSlots()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _email = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var now = DateTime.Now;
            var slots = await _db.BookingSlots
                        .Include(s => s.Offers)
                        .Include(s => s.RequestedPlayers)
                        .Where(s => s.Status == Model.GameSlotStatus.WAITING)
                        .ToListAsync();
            System.Console.WriteLine("Active Slot : " + slots.Count);

            var activeSlots = slots.Where(s => s.Date.Add(s.StartTime.ToTimeSpan()) > now).ToList();

            foreach (var slot in activeSlots)
            {
                await ProcessSlot(slot, now, _db, _email);
            }
        }

        private async Task ProcessSlot(BookingSlot slot, DateTime now, ApplicationDbContext db, IEmailService emailService)
        {
            System.Console.WriteLine($"Processing Slot : {slot.Id}");
            using var tx = await db.Database.BeginTransactionAsync();
            slot = await db.BookingSlots.Include(s => s.Offers).Include(s => s.RequestedPlayers)
                    .Where(s => s.Id == slot.Id).FirstOrDefaultAsync();
            if (slot == null || slot.Status != GameSlotStatus.WAITING)
            {
                System.Console.WriteLine("Slot is Null");
                return;
            }
            System.Console.WriteLine($"Get Specific Slot : {slot.Id}");
            var slotStart = slot.Date.Add(slot.StartTime.ToTimeSpan());
            System.Console.WriteLine($"Slot Start Time : {slotStart}");
            var remainingMinutes = (slotStart - now).TotalMinutes;
            System.Console.WriteLine($"Slot start Remaining Time : {remainingMinutes}");
            if (remainingMinutes <= 5)
            {
                System.Console.WriteLine($"No Time Left Lets Mark as book");
                await AssignToRequest(db, slot, emailService);
                await tx.CommitAsync();
                return;
            }
            var activeOffers = slot.Offers
                    .Where(o => o.Status == SlotOfferStatus.InProcess)
                    .OrderBy(o => o.PriorityOrder)
                    .FirstOrDefault();
            if (activeOffers == null)
            {
                await CreateNextOffer(db,slot,emailService);
                activeOffers = slot.Offers
                    .Where(o => o.Status == SlotOfferStatus.InProcess)
                    .OrderBy(o => o.PriorityOrder)
                    .FirstOrDefault();
            }
            // it run only for fist time
            if (activeOffers == null)
            {
                System.Console.WriteLine($"No Active Offere");
                System.Console.WriteLine("No Active User We Can Book For Requested user !");
                await AssignToRequest(db, slot, emailService); // i guess we have to book here 
                await tx.CommitAsync();
                return;
            }
            System.Console.WriteLine($"found active Offere");
            System.Console.WriteLine($"current : {now} , Expired : {activeOffers.CreatedAt.AddMinutes(5)}");
            if (now > activeOffers.CreatedAt.AddMinutes(5))
            {
                System.Console.WriteLine($"Offer {activeOffers.Id} Is Expired Not Send to next one");
                activeOffers.Status = SlotOfferStatus.Expired;
                activeOffers.ExpiredAt = now;
                await db.SaveChangesAsync();
                await CreateNextOffer(db, slot, emailService);
            }
            await db.SaveChangesAsync();
            await tx.CommitAsync();
        }

        private async Task AssignToRequest(ApplicationDbContext db, BookingSlot slot, IEmailService emailService)
        {
            System.Console.WriteLine($"Confirm Booking For Requested User : {slot.RequestBy}");
            slot.Status = GameSlotStatus.BOOKED;
            slot.BookedBy = slot.RequestBy;
            List<string> mailTo = new List<string>();
            foreach (var player in slot.RequestedPlayers)
            {
                var bookingPlayer = new BookingPlayer()
                {
                    PlayerId = player.PlayerId,
                    SlotId = slot.Id
                };
                User p = await db.Users.Where(u => u.Id == player.PlayerId && u.is_deleted == false).FirstOrDefaultAsync();
                if (p != null)
                    mailTo.Add(p.Email);
                await db.BookingPlayers.AddAsync(bookingPlayer);
            }
            foreach (var offer in slot.Offers.Where(o => o.Status == SlotOfferStatus.Pending))
            {
                offer.Status = SlotOfferStatus.Expired;
                offer.ExpiredAt = DateTime.UtcNow;
                db.SlotOffers.Update(offer);
            }

            UserGameState userGameState = await db.UserGameStates
                                    .Where((u) => u.GameId == slot.GameId && u.UserId == slot.RequestBy)
                                    .FirstOrDefaultAsync();
            if (userGameState == null)
                return;
            userGameState.GamePlayed += 1;
            db.UserGameStates.Update(userGameState);
            await db.SaveChangesAsync();
            System.Console.WriteLine("game state updated !");

            db.BookingSlots.Update(slot);
            await db.SaveChangesAsync();
            System.Console.WriteLine("Booking done!");

            User user = await db.Users.Where(u => u.Id == slot.RequestBy && u.is_deleted == false).FirstOrDefaultAsync();
            if (user != null)
            {
                await emailService.SendEmailAsync(user.Email, "Slot Booked", $"Your booking for slot {slot.Id} is confirmed.");
                foreach (var email in mailTo)
                {
                    await emailService.SendEmailAsync(email, "Slot Booked", $"Slot {slot.Id} is booked by {user.Email} user.");
                }
            }
            System.Console.WriteLine("Mail Sended to all");
        }

        private async Task CreateNextOffer(ApplicationDbContext db, BookingSlot slot, IEmailService emailService)
        {
            System.Console.WriteLine("Sending Offere to next player");
            var nextOrdere = slot.CurrentPriorityOrder + 1;
            var nextOfere = slot.Offers.
                    FirstOrDefault(o => o.PriorityOrder == nextOrdere && o.Status == SlotOfferStatus.Pending);
            if (nextOfere == null)
                return;
            nextOfere.Status = SlotOfferStatus.InProcess;
            nextOfere.CreatedAt = DateTime.Now;
            db.SlotOffers.Update(nextOfere);
            await db.SaveChangesAsync();

            slot.CurrentPriorityOrder = nextOrdere;
            db.BookingSlots.Update(slot);

            await db.SaveChangesAsync();
            User user = await db.Users.Where(u => u.Id == nextOfere.OffereTo).FirstOrDefaultAsync();
            if (user != null)
                await emailService.SendEmailAsync(user.Email, "Slot Offer", $"Your offer for slot {slot.Id} is now active. Please respond within 5 minutes.");
            System.Console.WriteLine("Offere Mail Sended !");
        }
    }
}