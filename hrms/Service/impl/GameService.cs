using AutoMapper;
using hrms.CustomException;
using hrms.Dto.Request.BookingSlot;
using hrms.Dto.Request.Game;
using hrms.Dto.Response.Game;
using hrms.Dto.Response.Other;
using hrms.Model;
using hrms.Repository;

namespace hrms.Service.impl
{
    public class GameService(
        IGameRepository _repository,
        IUserRepository _userRepository,
        IUserGameRepository _userGameRepository,
        IMapper _mapper) : IGameService
    {
        public async Task<GameSlotResponseDto> BookGameSlot(int gameId, int slotId, int userId, BookSlotRequestDto dto)
        {
            await _repository.GetGameSlotById(gameId, slotId);
            await _userRepository.GetByIdAsync(userId);
            await ValidateBookSlotRequest(gameId, slotId, userId, dto);
            GameSlot gameSlot = await _repository.BookGameSlot(gameId, slotId, userId, dto);
            return _mapper.Map<GameSlotResponseDto>(gameSlot);
        }

        private async Task ValidateBookSlotRequest(int gameId, int slotId, int userId, BookSlotRequestDto dto)
        {
            Game game = await _repository.GetGameById(gameId);
            if (game.MaxPlayer < dto.Players.Count + 1)
            {
                throw new InvalidOperationCustomException("Number of players exceeds the maximum allowed for this game.");
            }
            if (game.MinPlayer > dto.Players.Count + 1)
            {
                throw new InvalidOperationCustomException("Number of players is less than the minimum required for this game.");
            }
            if (!await _userGameRepository.IsUserInterestedInGame(userId, gameId))
            {
                throw new InvalidOperationCustomException("User is not already interested");
            }
            foreach (var player in dto.Players)
            {
                if (!await _userGameRepository.IsUserInterestedInGame(player, gameId))
                {
                    throw new InvalidOperationCustomException($"Player with ID {player} is not interested");
                }
            }
            if (dto.Players.Count < 1)
            {
                throw new InvalidOperationCustomException("At least one player must be added to book a slot.");
            }
            if (await _repository.IsUserAlreadyBookedInSlot(gameId, slotId, userId))
            {
                throw new InvalidOperationCustomException("User has already booked this slot.");
            }
            foreach (var player in dto.Players)
            {
                if (await _repository.IsUserAlreadyBookedInSlot(gameId, slotId, player))
                {
                    throw new InvalidOperationCustomException($"Player with ID {player} has already booked this slot.");
                }
            }
        }

        public async Task<GameSlotResponseDto> CancelGameSlot(int gameId, int slotId)
        {
            GameSlot slot = await _repository.GetGameSlotById(gameId, slotId);
            
            var slotStartDateTime = slot.Date.Add(slot.StartTime.ToTimeSpan());
            var timeUntilSlotStart = slotStartDateTime - DateTime.Now;
            
            if (timeUntilSlotStart.TotalMinutes <= 15)
            {
                throw new InvalidOperationCustomException("Cannot cancel booking. The slot starts in less than 15 minutes.");
            }
            
            GameSlotWaiting myWaitlistEntry = await _repository.GetUserWaitlistEntryForSlot(gameId, slotId, (int)slot.BookedById);
            await _userGameRepository.DecrementGamePlayed((int)slot.BookedById, gameId);
            myWaitlistEntry.IsCancelled = true;
            await _repository.UpdateGameSlotWaiting(myWaitlistEntry);
            slot.BookedById = null;
            slot.BookedBy = null;
            
            // Materialize the collection first to avoid "collection modified" error
            var playersToRemove = slot.Players.ToList();
            foreach (var player in playersToRemove)
            {
                await _userGameRepository.DecrementGamePlayed(player.PlayerId, gameId);
                await _repository.RemovePlayerFromGameSlot(player.Id);
            }
            slot.Status = GameSlotStatus.WAITING;
            GameSlot updatedSlot = await _repository.UpdateGameSlot(slot);
            return _mapper.Map<GameSlotResponseDto>(updatedSlot);
        }

        public async Task<GameResponseDto> CreateGame(GameCreateDto dto)
        {
            Game game = _mapper.Map<Game>(dto);
            Game createdGame = await _repository.CreateGame(game);
            return _mapper.Map<GameResponseDto>(createdGame);
        }

        public async Task<GameOperationWindowResponseDto> CreateGameOperationWindow(int gameId, GameOperationWindowCreateDto dto)
        {
            GameOperationWindow window = _mapper.Map<GameOperationWindow>(dto);
            await _repository.GetGameById(gameId);
            window.GameId = gameId;
            if (window.OperationalEndTime <= window.OperationalStartTime)
            {
                throw new InvalidOperationCustomException("Operational end time must be after start time.");
            }
            if (window.OperationalStartTime == window.OperationalEndTime)
            {
                throw new InvalidOperationCustomException("Operational start time and end time cannot be the same.");
            }
            if (await _repository.IsOperationWindowOverlap(gameId, window.OperationalStartTime, window.OperationalEndTime))
            {
                throw new InvalidOperationCustomException("Operation window overlaps with existing windows.");
            }
            GameOperationWindow createdWindow = await _repository.CreateGameOperationWindow(window);
            return _mapper.Map<GameOperationWindowResponseDto>(createdWindow);
        }

        public async Task<bool> DeleteGame(int gameId)
        {
            Game game = await _repository.GetGameById(gameId);
            await _repository.DeleteGame(game);
            return true;
        }

        public async Task<bool> DeleteGameOperationWindow(int gameId, int windowId)
        {
            GameOperationWindow window = await _repository.GetGameOperationWindowById(windowId);
            await _repository.DeleteGameOperationWindow(window);
            return true;
        }

        public async Task<List<GameOperationWindowResponseDto>> GetAllGameOperationWindows(int gameId)
        {
            List<GameOperationWindow> windows = await _repository.GetAllGameOperationWindows(gameId);
            return _mapper.Map<List<GameOperationWindowResponseDto>>(windows);
        }

        public async Task<PagedReponseDto<GameResponseDto>> GetAllGames(int pageNumber, int pageSize)
        {
            PagedReponseOffSet<Game> pagedGames = await _repository.GetAllGames(pageNumber, pageSize);
            return _mapper.Map<PagedReponseDto<GameResponseDto>>(pagedGames);
        }

        public async Task<List<GameSlotResponseDto>> GetAllGameSlots(int gameId, DateTime? startDate, DateTime? endDate)
        {
            List<GameSlot> slots = await _repository.GetAllGameSlots(gameId, startDate.Value, endDate.Value);
            return _mapper.Map<List<GameSlotResponseDto>>(slots);
        }

        public async Task<GameResponseWithSlot> GetGame(int gameId)
        {
            Game game = await _repository.GetGameById(gameId);
            GameResponseWithSlot response = _mapper.Map<GameResponseWithSlot>(game);
            return response;
        }

        public async Task<GameSlotDetailResponseDto> GetGameSlot(int gameId, int slotId)
        {
            GameSlot slot = await _repository.GetGameSlotById(gameId, slotId);
            return _mapper.Map<GameSlotDetailResponseDto>(slot);
        }

        public async Task<List<GameSlotWaitinglistResponseDto>> GetGameSlotWaitlist(int gameId, int slotId)
        {
            List<GameSlotWaiting> waitlistEntries = await _repository.GetGameSlotWaitlist(gameId, slotId);
            return _mapper.Map<List<GameSlotWaitinglistResponseDto>>(waitlistEntries);
        }

        public async Task<GameResponseDto> UpdateGame(int gameId, GameUpdateDto dto)
        {
            Game game = await _repository.GetGameById(gameId);
            Game updatedGame = UpdateGameFromDto(game, dto);
            updatedGame = await _repository.UpdateGame(updatedGame);
            return _mapper.Map<GameResponseDto>(updatedGame);
        }

        private Game UpdateGameFromDto(Game game, GameUpdateDto dto)
        {
            if (dto.Name != null)
                game.Name = dto.Name;
            if (dto.MaxPlayer != null)
                game.MaxPlayer = (int)dto.MaxPlayer;
            if (dto.MinPlayer != null)
                game.MinPlayer = (int)dto.MinPlayer;
            if (dto.Duration != null)
                game.Duration = (int)dto.Duration;
            if (dto.SlotAssignedBeforeMinutes != null)
                game.SlotAssignedBeforeMinutes = (int)dto.SlotAssignedBeforeMinutes;
            if (dto.SlotCreateForNextXDays != null)
                game.SlotCreateForNextXDays = (int)dto.SlotCreateForNextXDays;
            return game;
        }

        public async Task<bool> ToggleGameInterest(int gameId, int userId)
        {
            System.Console.WriteLine($"Toggling game interest for Game ID {gameId} and User ID {userId}");
            await _repository.GetGameById(gameId);
            await _userRepository.GetByIdAsync(userId);
            return await _userGameRepository.ToggleGameInterestStatus(userId, gameId);
        }

        public async Task<bool> IsUserInterested(int gameId, int userId)
        {
            bool isInterested = await _userGameRepository.IsUserInterestedInGame(userId, gameId);
            return isInterested;
        }

        public async Task<GameSlotWaitinglistResponseDto> CancelWaitingListEntry(int gameId, int slotId, int waitlistId)
        {
            GameSlotWaiting entry = await _repository.GetGameSlotWaitlistById(waitlistId);
            entry.IsCancelled = true;
            await _repository.UpdateGameSlotWaiting(entry);
            return _mapper.Map<GameSlotWaitinglistResponseDto>(entry);
        }
    }
}
