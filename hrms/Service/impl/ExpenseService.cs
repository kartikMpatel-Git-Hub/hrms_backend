using AutoMapper;
using hrms.CustomException;
using hrms.dto.request.Expense;
using hrms.Dto.Request.Category;
using hrms.Dto.Request.Expense;
using hrms.Dto.Response.Expense;
using hrms.Dto.Response.Expense.Category;
using hrms.Dto.Response.Other;
using hrms.Model;
using hrms.Repository;
using hrms.Utility;
using Microsoft.Extensions.Caching.Memory;

namespace hrms.Service.impl
{
    public class ExpenseService(
        IExpenseRepository _repository,
        IUserService _userService,
        IUserRepository _userRepository,
        ITravelRepository _travelRepository,
        IMapper _mapper,
        IEmailService _email,
        ICloudinaryService _cloudinary,
        INotificationRepository _notificationRepository,
        ILogger<ExpenseService> _logger,
        IMemoryCache _cache
    ) : IExpenseService
    {

        public async Task<ExpenseCategoryResponseDto> CreateExpenseCategory(ExpenseCategoryCreateDto dto)
        {
            if (await _repository.ExistExpenseCategory(dto.Category))
            {
                throw new ExistsCustomException($"Category : {dto.Category} Already Exists !");
            }
            ExpenseCategory expenseCategory = new ExpenseCategory()
            {
                Category = dto.Category,
                is_deleted = false
            };
            ExpenseCategory response = await _repository.CreateCategory(expenseCategory);
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.ExpenseCategories));
            _logger.LogInformation("Expense category '{Category}' created and cache version incremented", dto.Category);
            return _mapper.Map<ExpenseCategoryResponseDto>(response);
        }

        public async Task<ExpenseResponseDto> AddExpense(
            int travelId,
            int currentUserId,
            ExpenseCreateDto dto,
            List<IFormFile> files
            )
        {
            if (!await _travelRepository.UserExistsInTravel(travelId, currentUserId))
            {
                throw new InvalidOperationCustomException($"Traveler : {currentUserId}Not Found With Travel : {travelId}");
            }
            ExpenseCategory category = await _repository.GetCategoryById(dto.CategoryId);
            Travel travel = await _travelRepository.GetTravelById(travelId);
            User employee = await _userService.GetEmployee(currentUserId);
            decimal todaysExpense = dto.Amount + await _travelRepository.GetTodaysExpense(travelId, currentUserId, dto.ExpenseDate);
            if (dto.ExpenseDate > DateTime.Now)
            {
                throw new InvalidOperationCustomException("Expense Date can not be in Future !");
            }
            if (todaysExpense > travel.MaxAmountLimit)
            {
                throw new InvalidOperationCustomException($"You have reached Daily Expense Limit !(LIMIT {travel.MaxAmountLimit})");
            }
            if (travel.StartDate > DateTime.Now || travel.EndDate.AddDays(10) < DateTime.Now)
            {
                throw new InvalidOperationCustomException("Expense can not add before trip start and after 10 days of completed trip !");
            }
            if (dto.ExpenseDate > travel.EndDate || dto.ExpenseDate < travel.StartDate)
                throw new InvalidOperationCustomException("Invalid Expense Date. it must be between travel days !");
            Expense expense = new Expense()
            {
                TravelId = travel.Id,
                Travel = travel,
                TravelerId = employee.Id,
                Traveler = employee,
                Amount = dto.Amount,
                CategoryId = category.Id,
                Category = category,
                ExpenseDate = dto.ExpenseDate,
                Status = ExpenseStatus.PENDING,
                Details = dto.Details != null ? dto.Details : "Expense Added !",
                Remarks = dto.Remarks != null ? dto.Remarks : ""
            };
            Expense AddedExpense = await _repository.AddExpense(expense);

            List<ExpenseProof> proofs = new List<ExpenseProof>();

            foreach (IFormFile file in files)
            {
                ExpenseProof proof = new ExpenseProof()
                {
                    ExpenseId = AddedExpense.Id,
                    Expense = AddedExpense,
                    ProofDocumentUrl = await _cloudinary.UploadAsync(file),
                    DocumentType = file.ContentType,
                    Remakrs = "",
                };
                proofs.Add(await _repository.AddProof(proof));
            }
            AddedExpense.Proofs = proofs;
            User hr = await _userService.GetUserEntityById(travel.CreatedBy);

            await _email.SendEmailAsync(hr.Email, "Expense Added", $"""
                Employee with email {employee.Email} added expense of Amount : {expense.Amount}
                """);
            Notification notification = new Notification()
            {
                NotifiedTo = hr.Id,
                Title = "Expense Added",
                Description = $"Employee with email {employee.Email} added expense of Amount : {expense.Amount}",
                IsViewed = false,
                NotificationDate = DateTime.Now
            };
            await _notificationRepository.CreateNotification(notification);
            IncrementCacheVersion(CacheVersionKey.ForTravelExpenses(travelId));
            _logger.LogInformation("Expense added for travel {TravelId} by employee {EmployeeId}, amount {Amount}", travelId, currentUserId, dto.Amount);
            return _mapper.Map<ExpenseResponseDto>(AddedExpense);
        }

        public async Task<List<ExpenseCategoryResponseDto>> GetExpenseCategory()
        {
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.ExpenseCategories));
            var key = $"ExpenseCategories:version:{version}";
            if (_cache.TryGetValue(key, out List<ExpenseCategoryResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for expense categories (version {Version})", version);
                return cached;
            }
            List<ExpenseCategory> categories = await _repository.GetAllExpenseCategory();
            var result = _mapper.Map<List<ExpenseCategoryResponseDto>>(categories);
            _cache.Set(key, result, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved {Count} expense categories and cached with version {Version}", result.Count, version);
            return result;
        }

        public async Task<List<ExpenseResponseDto>> GetTravelTravelerExpense(int travelId, int travelerId)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForTravelExpenses(travelId));
            var key = $"TravelerExpenses:TravelId:{travelId}:TravelerId:{travelerId}:version:{version}";
            if (_cache.TryGetValue(key, out List<ExpenseResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for traveler expenses (version {Version})", version);
                return cached;
            }
            List<Expense> expenses = await _repository.GetAllTravelTravelerExpense(travelId, travelerId);
            var result = _mapper.Map<List<ExpenseResponseDto>>(expenses);
            _cache.Set(key, result, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Retrieved expenses for travel {TravelId} traveler {TravelerId} and cached with version {Version}", travelId, travelerId, version);
            return result;
        }

        public async Task<ExpenseResponseDto> ChangeExpenseStatus(int travelId, int travelerId, int expenseId, ExpenseStatusChangeDto dto)
        {
            Expense expense = await _repository.GetExpenseById(expenseId);
            if (dto.Status == ExpenseStatus.PENDING.ToString())
            {
                throw new InvalidOperationCustomException("updating Status Can not be Pending !");
            }
            if (expense.Status == ExpenseStatus.PENDING)
            {
                expense.Status = GetStatus(dto.Status);
                expense.Remarks = dto.Remarks != null ? dto.Remarks : "";
                expense.updated_at = DateTime.Now;
            }
            else
            {
                throw new InvalidOperationCustomException("Expense Review already given !");
            }
            Expense e = await _repository.UpdateExpenseStatus(expense);
            await NotifiedUserForExpenseStatusChange(e);
            IncrementCacheVersion(CacheVersionKey.ForTravelExpenses(travelId));
            _logger.LogInformation("Expense {ExpenseId} status changed to {Status} for travel {TravelId}", expenseId, dto.Status, travelId);
            return _mapper.Map<ExpenseResponseDto>(e);
        }

        private async Task NotifiedUserForExpenseStatusChange(Expense e)
        {
            var traveler = await _userRepository.GetById(e.TravelerId);
            await _email.SendEmailAsync(traveler.Email, "Expense Status Changed", $"""
                Your expense {e.Details} has been updated to status {e.Status}.
                """);
            Notification notification = new Notification()
            {
                NotifiedTo = traveler.Id,
                Title = "Expense Status Changed",
                Description = $"Your expense {e.Details} has been updated to status {e.Status}.",
                IsViewed = false,
                NotificationDate = DateTime.Now
            };
            await _notificationRepository.CreateNotification(notification);
            _logger.LogInformation("Notification created for user {UserId} for expense status change", traveler.Id);
            IncrementCacheVersion(CacheVersionKey.ForUserNotifications(e.TravelerId));
        }

        private void IncrementCacheVersion(string versionKey)
        {
            var current = _cache.Get<int>(versionKey);
            _cache.Set(versionKey, current + 1);
        }

        private ExpenseStatus GetStatus(string status)
        {
            switch (status)
            {
                case "APPROVED":
                    return ExpenseStatus.APPROVED;
                case "REJECTED":
                    return ExpenseStatus.REJECTED;
                default:
                    throw new NotFoundCustomException($"Expense status : {status} not found !");
            }
        }

        public async Task<PagedReponseDto<ExpenseResponseDto>> GetAllExpenses(int pageNumber, int pageSize, int currentUserId)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForTravelExpenses(currentUserId));
            var key = $"AllExpenses:UserId:{currentUserId}:pageNumber:{pageNumber}:pageSize:{pageSize}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<ExpenseResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for all expenses (version {Version})", version);
                return cached;
            }
            PagedReponseOffSet<Expense> expenses = await _repository.GetAllExpenses(pageNumber, pageSize, currentUserId);
            var result = _mapper.Map<PagedReponseDto<ExpenseResponseDto>>(expenses);
            _cache.Set(key, result, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Retrieved all expenses for user {UserId} and cached with version {Version}", currentUserId, version);
            return result;
        }

        public async Task<ExpenseResponseDto> UpdateExpense(int travelId, int expenseId, int currentUserId, ExpenseUpdateDto dto)
        {
            if (!await _travelRepository.UserExistsInTravel(travelId, currentUserId))
            {
                throw new InvalidOperationCustomException($"Traveler : {currentUserId}Not Found With Travel : {travelId}");
            }
            if (dto.CategoryId != null)
            {
                await _repository.GetCategoryById((int)dto.CategoryId);
            }
            Travel travel = await _travelRepository.GetTravelById(travelId);
            Expense expense = await _repository.GetExpenseById(expenseId);
            if (dto.Amount != null)
            {
                decimal todaysExpense = (decimal)dto.Amount + await _travelRepository.GetTodaysExpense(travelId, currentUserId, dto.ExpenseDate);
                if (dto.ExpenseDate == expense.ExpenseDate)
                {
                    todaysExpense -= expense.Amount;
                }
                if (todaysExpense > travel.MaxAmountLimit)
                {
                    throw new InvalidOperationCustomException($"You have reached Daily Expense Limit !(LIMIT {travel.MaxAmountLimit})");
                }
            }
            if (dto.ExpenseDate > DateTime.Now)
            {
                throw new InvalidOperationCustomException("Expense Date can not be in Future !");
            }
            if (travel.StartDate > DateTime.Now || travel.EndDate.AddDays(10) < DateTime.Now)
            {
                throw new InvalidOperationCustomException("Expense can not add before trip start and after 10 days of completed trip !");
            }
            if (dto.ExpenseDate > travel.EndDate || dto.ExpenseDate < travel.StartDate)
                throw new InvalidOperationCustomException("Invalid Expense Date. it must be between travel days !");

            Expense updatedExpense = UpdateExpenseEntity(expense, dto);
            Expense e = await _repository.UpdateExpense(updatedExpense);
            User hr = await _userRepository.GetById(travel.CreatedBy);
            await _email.SendEmailAsync(hr.Email, "Expense Updated", $"""
                    Employee with email {hr.Email} updated expense of Amount : {expense.Amount}
                    """);
            Notification notification = new Notification()
            {
                NotifiedTo = hr.Id,
                Title = "Expense Updated",
                Description = $"Employee with email {hr.Email} updated expense of Amount : {expense.Amount}",
                IsViewed = false,
                NotificationDate = DateTime.Now
            };
            await _notificationRepository.CreateNotification(notification);

            IncrementCacheVersion(CacheVersionKey.ForTravelExpenses(travelId));
            IncrementCacheVersion(CacheVersionKey.ForTravelExpenses(currentUserId));
            _logger.LogInformation("Updated expense {ExpenseId} for travel {TravelId} by user {UserId}", expenseId, travelId, currentUserId);

            return _mapper.Map<ExpenseResponseDto>(e);
        }

        private Expense UpdateExpenseEntity(Expense expense, ExpenseUpdateDto dto)
        {
            if (dto.Amount != null) expense.Amount = (decimal)dto.Amount;
            if (dto.CategoryId != null) expense.CategoryId = (int)dto.CategoryId;
            expense.ExpenseDate = (DateTime)dto.ExpenseDate;
            if (dto.Details != null) expense.Details = dto.Details;
            if (dto.Remarks != null) expense.Remarks = dto.Remarks;
            return expense;
        }
    }
}
