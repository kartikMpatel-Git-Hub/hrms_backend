using AutoMapper;
using hrms.CustomException;
using hrms.Dto.Request.Category;
using hrms.Dto.Request.Expense;
using hrms.Dto.Response.Expense;
using hrms.Dto.Response.Expense.Category;
using hrms.Model;
using hrms.Repository;

namespace hrms.Service.impl
{
    public class ExpenseService : IExpenseService
    {

        private readonly IExpenseRepository _repository;
        private readonly IUserService _userService;
        private readonly ITravelRepository _travelRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _email;
        private readonly ICloudinaryService _cloudinary;

        public ExpenseService(IEmailService email, IExpenseRepository repository, IMapper mapper, IUserService userService, ITravelRepository travelRepository,ICloudinaryService cloudinary)
        {
            _repository = repository;
            _userService = userService;
            _mapper = mapper;
            _email = email;
            _travelRepository = travelRepository;
            _cloudinary = cloudinary;
        }
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
            return _mapper.Map<ExpenseCategoryResponseDto>(response);
        }

        public async Task<ExpenseResponseDto> AddExpense(
            int travelId,
            int currentUserId,
            ExpenseCreateDto dto,
            List<IFormFile> files
            )
        {
            if(!await _travelRepository.UserExistsInTravel(travelId,currentUserId))
            {
                throw new InvalidOperationCustomException($"Traveler : {currentUserId}Not Found With Travel : {travelId}");
            }
            Travel travel = await _travelRepository.GetTravelById(travelId);
            if(dto.Amount > travel.MaxAmountLimit)
            {
                throw new InvalidOperationCustomException($"Expense Amount can not exide {travel.MaxAmountLimit}");
            }
            User employee = await _userService.GetEmployee(currentUserId);
            ExpenseCategory category = await _repository.GetCategoryById(dto.CategoryId);
            if (travel.StartDate > DateTime.Now || travel.EndDate.AddDays(10) < DateTime.Now)
            {
                throw new InvalidOperationCustomException("Expense can not add before trip start and after 10 days of completed trip !");
            }
            Expense expense = new Expense()
            {
                TravelId = travel.Id,
                Travel = travel,
                TravelerId = employee.Id,
                Traveler = employee,
                Amount = dto.Amount,
                CategoryId = category.Id,
                Category = category,
                Status = ExpenseStatus.PENDING,
                Details = dto.Details != null ? dto.Details : "Expense Added !",
                Remarks = dto.Remarks != null ? dto.Remarks : ""
            };
            Expense AddedExpense = await _repository.AddExpense(expense);
            
            List<ExpenseProof> proofs = new List<ExpenseProof>();
            
            foreach (IFormFile file in files) {
                ExpenseProof proof = new ExpenseProof()
                {
                    ExpenseId = AddedExpense.Id,
                    Expense = AddedExpense,
                    ProofDocumentUrl = await _cloudinary.UploadAsync(file),
                    DocumentType = file.ContentType,
                    Remakrs = ""
                };
                proofs.Add(await _repository.AddProof(proof));
            }
            AddedExpense.Proofs = proofs;
            User hr = await _userService.GetUserEntityById(travel.CreatedBy);

            await _email.SendEmailAsync(hr.Email, "Expense Added", $"""
                Employee with email {employee.Email} added expense of Amount : {expense.Amount}
                """);
            return _mapper.Map<ExpenseResponseDto>(AddedExpense);
        }

        public async Task<List<ExpenseCategoryResponseDto>> GetExpenseCategory()
        {
            List<ExpenseCategory> categories = await _repository.GetAllExpenseCategory();
            return _mapper.Map<List<ExpenseCategoryResponseDto>>(categories);
        }

        public async Task<List<ExpenseResponseDto>> GetTravelTravelerExpense(int travelId, int travelerId)
        {
            List<Expense> expenses = await _repository.GetAllTravelTravelerExpense(travelId, travelerId);
            return _mapper.Map<List<ExpenseResponseDto>>(expenses);
        }

        public async Task<ExpenseResponseDto> ChangeExpenseStatus(int travelId, int travelerId, int expenseId,ExpenseStatusChangeDto dto)
        {
            Expense expense = await _repository.GetExpenseById(expenseId);
            if(dto.Status == ExpenseStatus.PENDING.ToString())
            {
                throw new InvalidOperationCustomException("updating Status Can not be Pending !");
            }
            if(expense.Status == ExpenseStatus.PENDING)
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
            return _mapper.Map<ExpenseResponseDto>(e);
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
    }
}
