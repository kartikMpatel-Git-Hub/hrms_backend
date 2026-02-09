using CloudinaryDotNet;
using hrms.CustomException;
using hrms.Dto.Request.Category;
using hrms.Dto.Request.Expense;
using hrms.Dto.Response.Expense;
using hrms.Dto.Response.Expense.Category;
using hrms.Model;
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
        private readonly ICloudinaryService _cloudinary;

        public ExpenseController(IExpenseService service,ICloudinaryService cloudinary)
        {
            _service = service;
            _cloudinary = cloudinary;
        }

        [HttpPost("{TravelId}/document")]
        public async Task<IActionResult> AddDocument(int? TravelId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File Not Found !" });
            Console.WriteLine(file.FileName);
            var url = await _cloudinary.UploadAsync(file);
            return Ok(new { url });
        }

        [HttpPost("{TravelId}/expense")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddExpense(
            int? TravelId,
            ExpenseCreateDto dto
            )
        {
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

        [HttpPost("expense/category")]
        public async Task<IActionResult> AddCategory(ExpenseCategoryCreateDto? dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Category Not Found !" });
            ExpenseCategoryResponseDto response = await _service.CreateExpenseCategory(dto);
            return Ok(response);
        }
    }
}
