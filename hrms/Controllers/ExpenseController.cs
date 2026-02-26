using hrms.CustomException;
using hrms.dto.request.Expense;
using hrms.Dto.Request.Category;
using hrms.Dto.Request.Expense;
using hrms.Dto.Response.Expense;
using hrms.Dto.Response.Expense.Category;
using hrms.Dto.Response.Other;
using hrms.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace hrms.Controllers
{
    [Route("travel")]
    [ApiController]
    [Authorize]
    public class ExpenseController(IExpenseService _service, ITravelService _travelService, ILogger<ExpenseController> _logger) : Controller
    {
        [HttpPost("{TravelId}/expense")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "EMPLOYEE")]
        public async Task<IActionResult> AddExpense(
            int? TravelId,
            ExpenseCreateDto dto
            )
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            List<IFormFile> files = dto.Proofs;
            if (TravelId == null || dto == null
                || files == null || files.Count == 0
                )
            {
                return BadRequest("Required Resource Not Found !");
            }
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);

            int Id = (int)TravelId;
            ExpenseResponseDto respone = await _service.AddExpense(Id, CurrentUserId, dto, files);
            _logger.LogInformation("[{Method}] {Url} - Expense added to travel {TravelId} by user {UserId} successfully", Request.Method, Request.Path, TravelId, CurrentUserId);
            return Ok(respone);
        }

        [HttpPut("{TravelId}/expense/{ExpenseId}")]
        [Authorize(Roles = "EMPLOYEE")]
        public async Task<IActionResult> UpdateExpense(
            int? TravelId,
            int? ExpenseId,
            ExpenseUpdateDto dto
            )
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (TravelId == null || ExpenseId == null || dto == null)
                return BadRequest("Required Resource Not Found !");
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);

            int travelId = (int)TravelId;
            int expenseId = (int)ExpenseId;
            ExpenseResponseDto respone = await _service.UpdateExpense(travelId, expenseId, CurrentUserId, dto);
            _logger.LogInformation("[{Method}] {Url} - Expense {ExpenseId} in travel {TravelId} updated by user {UserId} successfully", Request.Method, Request.Path, ExpenseId, TravelId, CurrentUserId);
            return Ok(respone);
        }

        [HttpGet("all-expense")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetAllExpenses(
            int pageNumber = 1,
            int pageSize = 10
            )
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (pageNumber < 1)
                return BadRequest("Page Number must be greater than 0 !");
            if (pageSize < 1)
                return BadRequest("Page Size must be greater than 0 !");
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<ExpenseResponseDto> response = await _service.GetAllExpenses(pageNumber, pageSize, CurrentUserId);
            _logger.LogInformation("[{Method}] {Url} - Fetched all expenses for HR {UserId} successfully", Request.Method, Request.Path, CurrentUserId);
            return Ok(response);
        }


        [HttpGet("{TravelId}/traveler/{TravelerId}/expense")]
        public async Task<IActionResult> GetTravelTravelerExpense(
            int? TravelId,
            int? TravelerId
            )
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (TravelId == null || TravelerId == null)
                throw new NotFoundCustomException("Travel or Traveler Id not found !");
            int travelId = (int)TravelId;
            int travelerId = (int)TravelerId;
            List<ExpenseResponseDto> respone = await _service.GetTravelTravelerExpense(travelId, travelerId);
            _logger.LogInformation("[{Method}] {Url} - Fetched {Count} expenses for travel {TravelId} and traveler {TravelerId} successfully", Request.Method, Request.Path, respone.Count, TravelId, TravelerId);
            return Ok(respone);
        }

        [HttpGet("{TravelId}/expense")]
        [Authorize(Roles = "EMPLOYEE")]
        public async Task<IActionResult> GetEmployeeExpense(
            int? TravelId,
            int pageNumber = 1,
            int pageSize = 10
            )
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (TravelId == null)
                throw new NotFoundCustomException("Travel Id not found !");
            int travelId = (int)TravelId;
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int travelerId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);

            PagedReponseDto<ExpenseResponseDto> response = await _travelService.GetExpensesByTravelIdAndTravelerId(travelId, travelerId, pageSize, pageNumber);
            _logger.LogInformation("[{Method}] {Url} - Fetched expenses for employee {TravelerId} in travel {TravelId} successfully", Request.Method, Request.Path, travelerId, TravelId);
            return Ok(response);
        }


        [HttpPost("expense/category")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> AddCategory(ExpenseCategoryCreateDto? dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (dto == null)
                return BadRequest(new { message = "Category Not Found !" });
            ExpenseCategoryResponseDto response = await _service.CreateExpenseCategory(dto);
            _logger.LogInformation("[{Method}] {Url} - Expense category created successfully", Request.Method, Request.Path);
            return Ok(response);
        }

        [HttpGet("expense/category")]
        public async Task<IActionResult> GetExpenseCategory()
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            List<ExpenseCategoryResponseDto> response = await _service.GetExpenseCategory();
            _logger.LogInformation("[{Method}] {Url} - Fetched {Count} expense categories successfully", Request.Method, Request.Path, response.Count);
            return Ok(response);
        }


        [HttpPatch("{Travelid}/traveler/{TravelerId}/expense/{ExpenseId}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> ChangeExpenseStatus(
                int? TravelId,
                int? TravelerId,
                int? ExpenseId,
               ExpenseStatusChangeDto dto
            )
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (TravelId == null || TravelerId == null || ExpenseId == null)
                throw new NotFoundCustomException("Travel or Traveler or Expense Id not found !");
            int travelId = (int)TravelId;
            int travelerId = (int)TravelerId;
            int expenseId = (int)ExpenseId;
            ExpenseResponseDto respone = await _service.ChangeExpenseStatus(travelId, travelerId, expenseId, dto);
            _logger.LogInformation("[{Method}] {Url} - Expense {ExpenseId} status changed for travel {TravelId} and traveler {TravelerId} successfully", Request.Method, Request.Path, ExpenseId, TravelId, TravelerId);
            return Ok(respone);
        }
    }
}
