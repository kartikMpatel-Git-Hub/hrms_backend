using AutoMapper;
using Azure;
using hrms.CustomException;
using hrms.Dto.Request.Travel;
using hrms.Dto.Request.Travel.Document;
using hrms.Dto.Response.Expense;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Travel;
using hrms.Dto.Response.Travel.Document;
using hrms.Dto.Response.Traveler;
using hrms.Model;
using hrms.Repository;
using hrms.Utility;
using Microsoft.Extensions.Caching.Memory;

namespace hrms.Service.impl
{
    public class TravelService(
        IEmailService _email,
        ITravelRepository _repository,
        IMapper _mapper,
        IUserService _userService,
        ICloudinaryService _cloudinary,
        IMemoryCache _cache,
        ILogger<TravelService> _logger
    ) : ITravelService
    {

        public async Task AddTraveler(int TreavelId, TravelerAddDto dto)
        {
            Travel travel = await _repository.GetTravelById(TreavelId);
            _logger.LogInformation("Travel start date: {StartDate}, current date: {CurrentDate}", travel.StartDate, DateTime.Now);
            if(DateTime.Now.Date >= travel.StartDate.Date)
            {
                throw new InvalidOperationCustomException("You can not add traveler to this travel because the travel is already started or completed !");
            }
            _logger.LogInformation("Adding {Count} travelers to travel {TravelId}", dto.Travelers.Count, TreavelId);
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
                _logger.LogInformation("Traveler {TravelerId} added to travel {TravelId}", traveler.Id, TreavelId);
            }
            IncrementCacheVersion(CacheVersionKey.ForTravelTravelers(TreavelId));
            IncrementCacheVersion(CacheVersionKey.ForTravelInfo(TreavelId));
            foreach(var traveler in travelers){
                IncrementCacheVersion(CacheVersionKey.ForTravelerTravels(traveler.Id)); 
            }
             _logger.LogInformation("Cache versions incremented for travel {TravelId} after adding travelers", TreavelId);
        }

        public async Task<TravelResponseDto> CreateTravelAsync(TravelCreateDto Dto,int CurrentUser)
        {
            Travel travel = await CreateTravel(Dto,CurrentUser);
            Travel response = await _repository.CreateTravel(travel);
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.TravelDetails));
            _logger.LogInformation("Travel created with Id {TravelId} by user {UserId}", response.Id, CurrentUser);
            return _mapper.Map<TravelResponseDto>(response);
        }
        public async Task<TravelWithTravelerResponseDto> GetTravelersByTravelId(int TravelId) {
            var version = _cache.Get<int>(CacheVersionKey.ForTravelTravelers(TravelId));
            var key = $"TravelTravelers:TravelId:{TravelId}:version:{version}";
            if (_cache.TryGetValue(key, out TravelWithTravelerResponseDto cached))
            {
                _logger.LogDebug("Cache hit for travelers of travel {TravelId} (version {Version})", TravelId, version);
                return cached;
            }
            Travel travel = await _repository.GetTravelerByTravel(TravelId);
            var result = _mapper.Map<TravelWithTravelerResponseDto>(travel);
            _cache.Set(key, result, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Retrieved travelers for travel {TravelId} and cached with version {Version}", TravelId, version);
            return result;
        }
        public async Task<PagedReponseDto<TravelResponseDto>> GetHrCreatedTravels(int HrId, int PageSize, int PageNumber)
        {
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.TravelDetails));
            var key = $"HrTravels:HrId:{HrId}:PageSize:{PageSize}:PageNumber:{PageNumber}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<TravelResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for HR created travels (version {Version})", version);
                return cached;
            }
            PagedReponseOffSet<Travel> PageTravels = await _repository.GetTravelCreatedByHr(HrId, PageSize, PageNumber);
            var Response = _mapper.Map<PagedReponseDto<TravelResponseDto>>(PageTravels);
            _cache.Set(key, Response, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Retrieved HR {HrId} created travels and cached with version {Version}", HrId, version);
            return Response;
        }
        public async Task<TravelResponseDto> GetTravelByIdAsync(int TravelId)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForTravelInfo(TravelId));
            var key = $"TravelInfo:TravelId:{TravelId}:version:{version}";
            if (_cache.TryGetValue(key, out TravelResponseDto cached))
            {
                _logger.LogDebug("Cache hit for travel {TravelId} (version {Version})", TravelId, version);
                return cached;
            }
            Travel travel = await _repository.GetTravelById(TravelId);
            var result = _mapper.Map<TravelResponseDto>(travel);
            _cache.Set(key, result, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Retrieved travel {TravelId} and cached with version {Version}", TravelId, version);
            return result;
        }
        public async Task RemoveTravel(int TravelId)
        {
            await _repository.DeleteTravel(TravelId);
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.TravelDetails));
            IncrementCacheVersion(CacheVersionKey.ForTravelInfo(TravelId));
            IncrementCacheVersion(CacheVersionKey.ForTravelTravelers(TravelId));
            IncrementCacheVersion(CacheVersionKey.ForTravelDocuments(TravelId));
            IncrementCacheVersion(CacheVersionKey.ForTravelExpenses(TravelId));
            _logger.LogInformation("Travel {TravelId} removed and cache versions incremented", TravelId);
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
            if (dto.Location != null)
                Trav.Location = dto.Location;
            if(dto.MaxAmountLimit != null)
                Trav.MaxAmountLimit = (decimal)dto.MaxAmountLimit;
            Travel UpdatedTravel = await _repository.UpdateTravel(Trav);
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.TravelDetails));
            IncrementCacheVersion(CacheVersionKey.ForTravelInfo(TravelId));
            IncrementCacheVersion(CacheVersionKey.ForTravelTravelers(TravelId));
            foreach(var traveler in UpdatedTravel.Travelers)
            {
                IncrementCacheVersion(CacheVersionKey.ForTravelerTravels(traveler.TravelerId));
            }
            _logger.LogInformation("Travel {TravelId} updated and cache versions incremented", TravelId);
            return _mapper.Map<TravelResponseDto>(UpdatedTravel);
        }
        private async Task<Travel> CreateTravel(TravelCreateDto dto, int currentUserId)
        {
            User currentUser = await _userService.GetUserEntityById(currentUserId);
            Travel travel = new Travel()
            {
                Title = dto.Title,
                Desciption = dto.Description,
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

        public async Task<TravelDocumentResponseDto> AddTravelDocument(int travelId, int travelerId, int currentUserId, TravelDocumentCreateDto dto)
        {
            Travel travel = await _repository.GetTravelById(travelId);
            User currentUser = await _userService.GetUserEntityById(currentUserId);
            User traveler = await _repository.GetTravelerByTravelId(travelId, travelerId);
            
            TravelDocument document = new TravelDocument()
            {
                TravelerId = traveler.Id,
                Traveler = traveler,
                TravelId = travel.Id,
                Travell = travel,
                UploadedBy = currentUser.Id,
                Uploader = currentUser,
                DocumentName = dto.DocumentName,
                DocumentType = dto.Document.ContentType,
                DocumentUrl = await _cloudinary.UploadAsync(dto.Document)
            };
            TravelDocument travelDocument = await _repository.AddTravelDocument(document);
            IncrementCacheVersion(CacheVersionKey.ForTravelDocuments(travelId));
            _logger.LogInformation("Travel document added for travel {TravelId} traveler {TravelerId} by user {UserId}", travelId, travelerId, currentUserId);
            return _mapper.Map<TravelDocumentResponseDto>(travelDocument);
        }

        public async Task<List<TravelDocumentResponseDto>> GetTravelDocument(int travelId, int travelerId, int userId)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForTravelDocuments(travelId));
            var key = $"TravelDocuments:TravelId:{travelId}:TravelerId:{travelerId}:UserId:{userId}:version:{version}";
            if (_cache.TryGetValue(key, out List<TravelDocumentResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for travel documents (version {Version})", version);
                return cached;
            }
            List<TravelDocument> documents = await _repository.GetTravelDocuments(travelId, travelerId, userId);
            var result = _mapper.Map<List<TravelDocumentResponseDto>>(documents);
            _cache.Set(key, result, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Retrieved travel documents for travel {TravelId} traveler {TravelerId} and cached with version {Version}", travelId, travelerId, version);
            return result;
        }

        public async Task<PagedReponseDto<TravelResponseDto>> GetEmployeeTravels(int currentUserId, int pageSize, int pageNumber)
        {
            User employee = await _userService.GetEmployee(currentUserId);
            var version = _cache.Get<int>(CacheVersionKey.ForTravelerTravels(employee.Id));
            var key = $"EmployeeTravels:EmployeeId:{employee.Id}:pageSize:{pageSize}:pageNumber:{pageNumber}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<TravelResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for employee travels (version {Version})", version);
                return cached;
            }
            PagedReponseOffSet<Travel> PageTravels = await _repository.GetEmployeeTravels(employee.Id, pageSize, pageNumber);
            var Response = _mapper.Map<PagedReponseDto<TravelResponseDto>>(PageTravels);
            _cache.Set(key, Response, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Retrieved employee {EmployeeId} travels and cached with version {Version}", employee.Id, version);
            return Response;
        }

        public async Task<TravelerDto> AddTraveler(int TravelId, int TravelerId)
        {
            Travel travel = await _repository.GetTravelById(TravelId);
            _logger.LogInformation("Travel start date: {StartDate}, current date: {CurrentDate}", travel.StartDate, DateTime.Now);
            if(DateTime.Now.Date >= travel.StartDate.Date)
            {
                throw new InvalidOperationCustomException("You can not add traveler to this travel because the travel is already started or completed !");
            }
            if(await _repository.UserExistsInTravel(TravelId, TravelerId))
            {
                throw new ExistsCustomException("Traveler Already Exist in Travel !");
            }
            User traveler = await _userService.GetUserEntityById(TravelerId);
            Traveler t = new Traveler()
            {
                TravelId = travel.Id,
                Travel = travel,
                TravelerId = traveler.Id,
                Travelerr = traveler,
                is_deletd = false
            };
            Traveler AddedTraveler = await _repository.AddTraveler(t);
            await _email.SendEmailAsync(traveler.Email, "Your Traveling Booking !", $"Your Traveling is Booked from {travel.StartDate} to {travel.EndDate} for a {travel.Location}");
            IncrementCacheVersion(CacheVersionKey.ForTravelTravelers(TravelId));
            IncrementCacheVersion(CacheVersionKey.ForTravelInfo(TravelId));
            IncrementCacheVersion(CacheVersionKey.ForTravelerTravels(traveler.Id)); 
             _logger.LogInformation("Cache versions incremented for travel {TravelId} after adding traveler {TravelerId}", TravelId, TravelerId);
             _logger.LogInformation("Traveler {TravelerId} added to travel {TravelId}", TravelerId, TravelId);      
            _logger.LogInformation("Traveler {TravelerId} added to travel {TravelId}", TravelerId, TravelId);
            return _mapper.Map<TravelerDto>(AddedTraveler);
        }

        public async Task<PagedReponseDto<TravelResponseDto>> GetTravelsByTravelerId(int travelerId, int pageSize, int pageNumber)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForTravelerTravels(travelerId));
            var key = $"TravelerTravels:TravelerId:{travelerId}:pageSize:{pageSize}:pageNumber:{pageNumber}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<TravelResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for traveler travels (version {Version})", version);
                return cached;
            }
            PagedReponseOffSet<Travel> PageTravels = await _repository.GetTravelsByTravelerId(travelerId, pageSize, pageNumber);
            var result = _mapper.Map<PagedReponseDto<TravelResponseDto>>(PageTravels);
            _cache.Set(key, result, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Retrieved travels for traveler {TravelerId} and cached with version {Version}", travelerId, version);
            return result;
        }

        public async Task<PagedReponseDto<ExpenseResponseDto>> GetExpensesByTravelIdAndTravelerId(int travelId, int travelerId, int pageSize, int pageNumber)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForTravelExpenses(travelId));
            var key = $"TravelExpenses:TravelId:{travelId}:TravelerId:{travelerId}:pageSize:{pageSize}:pageNumber:{pageNumber}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<ExpenseResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for travel expenses (version {Version})", version);
                return cached;
            }
            PagedReponseOffSet<Expense> PageExpenses = await _repository.GetExpensesByTravelIdAndTravelerId(travelId, travelerId, pageSize, pageNumber);
            var result = _mapper.Map<PagedReponseDto<ExpenseResponseDto>>(PageExpenses);
            _cache.Set(key, result, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Retrieved expenses for travel {TravelId} traveler {TravelerId} and cached with version {Version}", travelId, travelerId, version);
            return result;
        }

        public async Task<PagedReponseDto<TravelDocumentResponseDto>> GetDocumentsByTravelIdAndTravelerId(int travelId, int travelerId, int pageSize, int pageNumber)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForTravelDocuments(travelId));
            var key = $"TravelDocsPaged:TravelId:{travelId}:TravelerId:{travelerId}:pageSize:{pageSize}:pageNumber:{pageNumber}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<TravelDocumentResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for travel documents paged (version {Version})", version);
                return cached;
            }
            PagedReponseOffSet<TravelDocument> PageDocuments = await _repository.GetDocumentsByTravelIdAndTravelerId(travelId, travelerId, pageSize, pageNumber);
            var result = _mapper.Map<PagedReponseDto<TravelDocumentResponseDto>>(PageDocuments);
            _cache.Set(key, result, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Retrieved documents for travel {TravelId} traveler {TravelerId} and cached with version {Version}", travelId, travelerId, version);
            return result;
        }

        private void IncrementCacheVersion(string versionKey)
        {
            var current = _cache.Get<int>(versionKey);
            _cache.Set(versionKey, current + 1);
        }
    }
}
