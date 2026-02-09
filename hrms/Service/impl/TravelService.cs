using AutoMapper;
using hrms.CustomException;
using hrms.Dto.Request.Category;
using hrms.Dto.Request.Expense;
using hrms.Dto.Request.Travel;
using hrms.Dto.Response.Expense;
using hrms.Dto.Response.Expense.Category;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Travel;
using hrms.Dto.Response.User;
using hrms.Model;
using hrms.Repository;
using System.Threading.Tasks;

namespace hrms.Service.impl
{
    public class TravelService : ITravelService
    {
        private readonly ITravelRepository _repository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IEmailService _email;

        public TravelService(IEmailService email, ITravelRepository repository,IMapper mapper,IUserService userService)
        {
            _repository = repository;
            _userService = userService;
            _mapper = mapper;
            _email = email;
        }

        public async Task AddTraveler(int TreavelId, TravelerAddDto dto)
        {
            Travel travel = await _repository.GetTravelById(TreavelId);
            List<User> travelers = new List<User>();
            foreach (var traveler in dto.Travelers)
            {
                if(!await _repository.UserExistsInTravel(TreavelId, traveler))
                {
                    User user = await _userService.GetUserEntityById(traveler);
                    travelers.Add(user);
                }
            }
            foreach(var traveler in travelers)
            {
                Traveler t = new Traveler()
                {
                    TravelId = travel.Id,
                    Travel = travel,
                    TravelerId = traveler.Id,
                    Travelerr = traveler,
                    is_deletd = false
                };
                await _repository.AddTraveler(t);
                await _email.SendEmailAsync(traveler.Email, "Your Traveling Booking !", $"Your Traveling is Booked from {travel.StartDate} to {travel.EndDate} for a {travel.Location}");
            }
        }

        public async Task<TravelResponseDto> CreateTravelAsync(TravelCreateDto Dto,int CurrentUser)
        {
            Travel travel = await CreateTravel(Dto,CurrentUser);
            Travel response = await _repository.CreateTravel(travel);
            return _mapper.Map<TravelResponseDto>(response);
        }
        public async Task<TravelWithTravelerResponseDto> GetTravelersByTravelId(int TravelId) {
            Travel travel = await _repository.GetTravelerByTravel(TravelId);
            return _mapper.Map<TravelWithTravelerResponseDto>(travel);
        }
        public async Task<PagedReponseDto<TravelResponseDto>> GetHrCreatedTravels(int HrId, int PageSize, int PageNumber)
        {
            PagedReponseOffSet<Travel> PageTravels = await _repository.GetTravelCreatedByHr(HrId, PageSize, PageNumber);
            var Response = _mapper.Map<PagedReponseDto<TravelResponseDto>>(PageTravels);
            return Response;
        }

        public async Task<TravelResponseDto> GetTravelByIdAsync(int TravelId)
        {
            Travel travel = await _repository.GetTravelById(TravelId);
            return _mapper.Map<TravelResponseDto>(travel);
        }

        public async Task RemoveTravel(int TravelId)
        {
            await _repository.DeleteTravel(TravelId);
        }

        public async Task<TravelResponseDto> UpdateTravelById(int TravelId, TravelUpdateDto dto)
        {
            Travel Trav = await _repository.GetTravelById(TravelId);
            if (dto == null)
                return _mapper.Map<TravelResponseDto>(Trav);
            if(dto.Title != null)
                Trav.Title = dto.Title;
            if(dto.Desciption != null)
                Trav.Desciption = dto.Desciption;
            if (dto.StartDate != null)
                Trav.StartDate = (DateTime)dto.StartDate;
            if (dto.EndDate != null)
                Trav.EndDate = (DateTime)dto.EndDate;
            if (dto.Location != null)
                Trav.Location = dto.Location;
            if(dto.MaxAmountLimit != null)
                Trav.MaxAmountLimit = (decimal)dto.MaxAmountLimit;
            Travel UpdatedTravel = await _repository.UpdateTravel(Trav);
            return _mapper.Map<TravelResponseDto>(UpdatedTravel);
        }

        private async Task<Travel> CreateTravel(TravelCreateDto dto, int currentUserId)
        {
            User currentUser = await _userService.GetUserEntityById(currentUserId);
            Travel travel = new Travel()
            {
                Title = dto.Title,
                Desciption = dto.Desciption,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Location = dto.Location,
                MaxAmountLimit = dto.MaxAmountLimit,
                CreatedBy = currentUser.Id,
                Creater = currentUser,
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
                is_deleted = false,
            };

            return travel;
        }

        

        // media part is pending

    }
}
