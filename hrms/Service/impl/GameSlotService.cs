
using System.Text;
using AutoMapper;
using hrms.CustomException;
using hrms.Dto.Request.BookingSlot;
using hrms.Dto.Response.BookingSlot;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.User;
using hrms.Model;
using hrms.Repository;

namespace hrms.Service.impl
{
    public class GameSlotService
        (
            IGameBookingRepository _repository,
            IUserRepository _userRepository,
            IGameRepository _gameRepository,
            IUserGameRepository _userGameRpository,
            IMapper _mapper,
            IEmailService _emailService,
            INotificationRepository _notificationRepository
        )
        : IGameSlotService
    {
        public async Task<BookingSlotResponseDto> BookSlot(int bookingSlotId, int currentUserId, BookSlotRequestDto dto)
        {
            User user = await _userRepository.GetByIdAsync( currentUserId );
            BookingSlot slot = await _repository.GetSlot( bookingSlotId );
            await CheckValidations(user,slot,dto);
            List<User> availablePlayers = await AvailablePlayers(dto.Players);
            slot.BookedBy = user.Id;
            slot.Booked = user;
            slot.Status = GameSlotStatus.BOOKED;
            foreach (var player in availablePlayers) {
                BookingPlayer bookingPlayer = new BookingPlayer()
                {
                    SlotId = slot.Id,
                    PlayerId = player.Id,
                    Slot = slot,
                    Player = player
                };
                slot.Players.Add( bookingPlayer );
            }
            BookingSlot bookedSlot = await _repository.BookSlot(slot);
            await SendEmailAndNotification(user);
            await UpdateUserGameState(slot);
            return _mapper.Map<BookingSlotResponseDto>(bookedSlot);
        }

        private async Task CheckValidations(User user, BookingSlot slot, BookSlotRequestDto dto)
        {
            if (slot.Game.MaxPlayer < dto.Players.Count || slot.Game.MinPlayer > dto.Players.Count)
            {
                throw new InvalidOperationCustomException($"Number of Players should be between {slot.Game.MinPlayer} and {slot.Game.MaxPlayer} !");
            }
            if(slot.Status != GameSlotStatus.AVAILABLE)
            {
                throw new InvalidOperationCustomException($"Slot is Already Booked !");
            }
            if(!await _userGameRpository.IsUserInterestedInGame(user.Id, slot.GameId))
            {
                throw new InvalidOperationCustomException($"You are not interested in this game !");
            }
            if (!await IsBookingPossible(user,slot))
            {
                throw new InvalidOperationCustomException($"You can not Book Slot Until your next Cycle start !");
            }
        }

        private async Task UpdateUserGameState(BookingSlot slot)
        {
            User booker = slot.Booked;
            UserGameState userGameState = await _userGameRpository.GetUserGameState(booker.Id, slot.GameId);
            userGameState.GamePlayed++;
            await _userGameRpository.UpdateuserState(userGameState);
        }

        private async Task SendEmailAndNotification(User to)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Hi {to.FullName},");
            stringBuilder.AppendLine($"Your Game Slot has been booked successfully!");
            await _emailService.SendEmailAsync(to.Email, "Game Slot Booked", stringBuilder.ToString());
            Notification notification = new Notification()
            {
                NotifiedTo = to.Id,
                Notified = to,
                Title = "Game Slot Booked",
                Description = "Your Game Slot has been booked successfully!",
                IsViewed = false,
                NotificationDate = DateTime.Now
            };
            await _notificationRepository.CreateNotification(notification); 
        }

        private async Task<bool> IsBookingPossible(User user, BookingSlot slot)
        {

            

            List<UserGameInterest> interested =
                    await _userGameRpository.GetInterestedPlayers(slot.GameId);

            if (interested == null || interested.Count == 0)
                return true;

            List<UserGameState> states =
                await _userGameRpository.GetUsersGameStates(slot.GameId);
            
            foreach (var intUser in interested)
            {
                var existing = states.FirstOrDefault(s => s.UserId == intUser.UserId);
                if (existing == null)
                {
                    var newState = new UserGameState()
                    {
                        GameId = slot.GameId,
                        UserId = intUser.UserId,
                        Game = slot.Game,
                        User = intUser.User,
                        GamePlayed = 0,
                        LastPayledAt = DateTime.Now
                    };
                    await _userGameRpository.CreateUserState(newState);
                    states.Add(newState);
                }
            }

            UserGameState currentState = states.First(s => s.UserId == user.Id);

            int minPlayed = states.Min(s => s.GamePlayed);

            if (currentState.GamePlayed > minPlayed)
            {
                return false;
            }

            return true;

        }

        private async Task<List<User>> AvailablePlayers(List<int> players)
        {
            List<User> users = new List<User>();
            foreach (var player in players)
            {
                users.Add(await _userRepository.GetById(player));
            }
            return users;
        }

        public async Task<BookingSlotResponseDto> CreateSlot(BookingSlotCreateDto dto)
        {
            BookingSlot newSlot = _mapper.Map<BookingSlot>(dto);
            Game game = await _gameRepository.GetGameById(dto.GameId);
            newSlot.Game = game;
            BookingSlot bookingSlot = await _repository.CreateSlot(newSlot);
            return _mapper.Map<BookingSlotResponseDto>(bookingSlot);
        }

        public Task<BookingSlotResponseDto> DeleteSlot(int slotId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BookingSlotResponseDto>> GetBookingSlots(int gameId,DateTime from, DateTime to)
        {
            List<BookingSlot> slots =  await _repository.GetSlots(gameId, from, to);
            return _mapper.Map<List<BookingSlotResponseDto>>(slots);
        }

        public async Task<BookingSlotWithPlayerResponseDto> GetSlot(int slotId)
        {
            BookingSlot slot = await _repository.GetSlot(slotId);
            return _mapper.Map<BookingSlotWithPlayerResponseDto>(slot);
        }

        public async Task RemovePlayer(int bookingId)
        {
            await _repository.RemovePlayer(bookingId);
        }

        public async Task<PagedReponseDto<UserResponseDto>> GetAvailablePlayers(int gameId, string? key, int pageSize, int pageNumber)
        {
            PagedReponseOffSet<User> users = await _userGameRpository.GetAvailablePlayers(gameId, key, pageSize, pageNumber);
            return _mapper.Map<PagedReponseDto<UserResponseDto>>(users);
        }
    }
}
