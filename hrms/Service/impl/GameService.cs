using AutoMapper;
using hrms.CustomException;
using hrms.Dto.Request.Game;
using hrms.Dto.Request.Game.GameSlot;
using hrms.Dto.Response.Game;
using hrms.Dto.Response.Game.GameSlot;
using hrms.Dto.Response.Other;
using hrms.Model;
using hrms.Repository;

namespace hrms.Service.impl
{
    public class GameService(
        IGameRepository _repository,
        IMapper _mapper) : IGameService
    {
        public async Task<GameResponseDto> CreateGame(GameCreateDto dto)
        {
            if(dto.MaxPlayer < dto.MinPlayer)
            {
                throw new InvalidOperationCustomException("Min Number of player should be less or equal to Max player !");
            }
            Game newGame = _mapper.Map<Game>( dto );
            newGame = await _repository.CreateGame(newGame);
            return _mapper.Map<GameResponseDto>( newGame );
        }

        public async Task<GameSlotResponseDto> CreateGameSlot(int gameId, GameSlotCreateDto dto)
        {
            if(await _repository.isSlotExist(dto.StartTime,dto.EndTime))
            {
                throw new InvalidOperationCustomException("Slot alredy exists on request timing !");
            }
            if(dto.StartTime > dto.EndTime)
            {
                throw new InvalidOperationCustomException("end time must be grater than start time !");
            }
            Game game = await _repository.GetGameById(gameId);
            GameSlot newSlot = _mapper.Map<GameSlot>( dto );
            newSlot.GameId = gameId;
            newSlot.Game = game;
            newSlot.StartTime = newSlot.StartTime;
            newSlot = await _repository.CreateGameSlot(newSlot);
            return _mapper.Map<GameSlotResponseDto>( newSlot );
        }

        public async Task DeleteGame(int gameId)
        {
            Game game = await _repository.GetGameById(gameId);
            await _repository.RemoveGame(game);
        }

        public async Task DeleteGameSlot(int gameSlotId)
        {
            GameSlot gameSlot = await _repository.GetGameSlotById(gameSlotId);
            await _repository.RemoveGameSlot(gameSlot);
        }

        public async Task<PagedReponseDto<GameResponseDto>> GetAllGames(int pageNumber, int pageSize)
        {
            PagedReponseOffSet<Game> data = await _repository.GetAllGames(pageNumber, pageSize);
            return _mapper.Map<PagedReponseDto<GameResponseDto>>( data );
        }

        public async Task<GameResponseWithSlot> GetGame(int gameId)
        {
            Game game = await _repository.GetGameById(gameId);
            return _mapper.Map<GameResponseWithSlot>(game);
        }

        public async Task<GameResponseDto> UpdateGame(int gameId, GameUpdateDto dto)
        {
            Game updatedGame = await Update(gameId, dto);
            updatedGame = await _repository.UpdateGame(updatedGame);
            return _mapper.Map<GameResponseDto>(updatedGame);
        }

        public async Task<GameSlotResponseDto> UpdateGameSlot(int gameSlotId, GameSlotUpdateDto dto)
        {
            if(await _repository.isSlotExist(dto.StartTime, dto.EndTime))
            {
                throw new InvalidOperationCustomException("Slot alredy exists on request timing !");
            }
            GameSlot updatedGameSlot = await Update(gameSlotId, dto);
            updatedGameSlot = await _repository.UpdateGameSlot(updatedGameSlot);
            return _mapper.Map<GameSlotResponseDto>(updatedGameSlot);
        }

        private async Task<Game> Update(int gameId, GameUpdateDto dto)
        {
            Game game = await _repository.GetGameById(gameId);
            if(dto.Name != null)
                game.Name = dto.Name;
            if (dto.MaxPlayer != null && dto.MinPlayer != null)
            {
                if(dto.MaxPlayer >= dto.MinPlayer)
                {
                    game.MaxPlayer = (int)dto.MaxPlayer;
                    game.MinPlayer = (int)dto.MinPlayer;
                }
                else
                {
                    throw new InvalidOperationCustomException("Min number of Player can not more than Max number of Player");
                }
            }
            else
            {
                if (dto.MaxPlayer != null)
                {
                    if (game.MinPlayer < dto.MaxPlayer)
                        game.MaxPlayer = (int)dto.MaxPlayer;
                    else
                        throw new InvalidOperationCustomException("Min number of Player can not more than Max number of Player");
                }
                if (dto.MinPlayer != null)
                {
                    if (game.MaxPlayer > dto.MinPlayer)
                        game.MinPlayer = (int)dto.MinPlayer;
                    else
                        throw new InvalidOperationCustomException("Min number of Player can not more than Max number of Player");
                } 
            }
            return game;
        }

        private async Task<GameSlot> Update(int gameSlotId,GameSlotUpdateDto dto)
        {
            GameSlot gameSlot = await _repository.GetGameSlotById(gameSlotId);
            if(dto.StartTime != null && dto.EndTime != null)
            {
                if(dto.StartTime < dto.EndTime)
                {
                    gameSlot.StartTime = (TimeOnly)dto.StartTime;
                    gameSlot.EndTime = (TimeOnly)dto.EndTime;
                }
                else
                {
                    throw new InvalidOperationCustomException("start time must be less than end time !");
                }
            }
            else
            {
                if(dto.StartTime != null)
                {
                    if (dto.StartTime < gameSlot.EndTime)
                        gameSlot.StartTime = (TimeOnly)dto.StartTime.AddMinutes(1);
                    else
                        throw new InvalidOperationCustomException("start time must be less than end time !");
                }
                if (dto.EndTime != null)
                {
                    if (dto.EndTime > gameSlot.StartTime)
                        gameSlot.EndTime = (TimeOnly)dto.EndTime;
                    else
                        throw new InvalidOperationCustomException("end time must be grater than start time !");
                }
            }
            return gameSlot;
        }
    }
}
