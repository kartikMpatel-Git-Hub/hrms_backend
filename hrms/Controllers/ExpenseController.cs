using hrms.CustomException;
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
    public class ExpenseController : Controller
    {
        private readonly IExpenseService _service;
        //private readonly ICloudinaryService _cloudinary;

        public ExpenseController(IExpenseService service
            //, ICloudinaryService cloudinary
            )
        {
            _service = service;
            //_cloudinary = cloudinary;
        }


        [HttpPost("{TravelId}/expense")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "EMPLOYEE")]
        public async Task<IActionResult> AddExpense(
            int? TravelId,
            ExpenseCreateDto dto
            )
        {
            Console.WriteLine("Adding New Expense !");
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
            return Ok(respone);
        }

        [HttpGet("all-expense")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetAllExpenses(
            int pageNumber = 1,
            int pageSize = 10
            )
        {
            if (pageNumber < 1)
                return BadRequest("Page Number must be greater than 0 !");
            if (pageSize < 1)
                return BadRequest("Page Size must be greater than 0 !");
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<ExpenseResponseDto> response = await _service.GetAllExpenses(pageNumber, pageSize, CurrentUserId);
            return Ok(response);
        }


        [HttpGet("{TravelId}/traveler/{TravelerId}/expense")]
        public async Task<IActionResult> GetTravelTravelerExpense(
            int? TravelId,
            int? TravelerId
            )
        {
            if (TravelId == null || TravelerId == null)
                throw new NotFoundCustomException("Travel or Traveler Id not found !");
            int travelId = (int)TravelId;
            int travelerId = (int)TravelerId;
            List<ExpenseResponseDto> respone = await _service.GetTravelTravelerExpense(travelId, travelerId);
            return Ok(respone);
        }

        [HttpGet("{TravelId}/expense")]
        [Authorize(Roles = "EMPLOYEE")]
        public async Task<IActionResult> GetEmployeeExpense(
            int? TravelId
            )
        {
            if (TravelId == null)
                throw new NotFoundCustomException("Travel Id not found !");
            int travelId = (int)TravelId;
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int travelerId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);

            List<ExpenseResponseDto> respone = await _service.GetTravelTravelerExpense(travelId, travelerId);
            return Ok(respone);
        }


        [HttpPost("expense/category")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> AddCategory(ExpenseCategoryCreateDto? dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Category Not Found !" });
            ExpenseCategoryResponseDto response = await _service.CreateExpenseCategory(dto);
            return Ok(response);
        }

        [HttpGet("expense/category")]
        public async Task<IActionResult> GetExpenseCategory()
        {
            List<ExpenseCategoryResponseDto> response = await _service.GetExpenseCategory();
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
            if (TravelId == null || TravelerId == null || ExpenseId == null)
                throw new NotFoundCustomException("Travel or Traveler or Expense Id not found !");
            int travelId = (int)TravelId;
            int travelerId = (int)TravelerId;
            int expenseId = (int)ExpenseId;
            ExpenseResponseDto respone = await _service.ChangeExpenseStatus(travelId, travelerId, expenseId, dto);
            return Ok(respone);
        }
    }
}
