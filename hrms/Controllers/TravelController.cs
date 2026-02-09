using hrms.CustomException;
using hrms.Dto.Request.Category;
using hrms.Dto.Request.Expense;
using hrms.Dto.Request.Travel;
using hrms.Dto.Response.Expense;
using hrms.Dto.Response.Expense.Category;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Travel;
using hrms.Dto.Response.User;
using hrms.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Security.Claims;

namespace hrms.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TravelController : Controller
    {
        private readonly ITravelService _service;

        public TravelController(ITravelService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> CreateTravel(TravelCreateDto dto)
        {
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            TravelResponseDto response = await _service.CreateTravelAsync(dto, CurrentUserId);
            return Ok(response);
        }

        [HttpPost("{TravelId}/travelers")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> AddTravelers(int? TravelId,TravelerAddDto Dto)
        {
            if(TravelId == null || Dto == null)
            {
                throw new NotFoundCustomException($"Travel Id or Request Body Not Found !");
            }
            int Id = (int)TravelId;
            await _service.AddTraveler(Id, Dto);
            return Ok("Traveler's Added Succesfully !");
        }

        [HttpPut("{TravelId}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> UpdateTravel(int? TravelId, TravelUpdateDto dto)
        {
            if (TravelId == null ||  dto == null)
            {
                return BadRequest(new {message = "TravelId or Updated field not Found !"});
            }
            int Id = (int)TravelId;
            TravelResponseDto response = await _service.UpdateTravelById(Id, dto);
            return Ok(response);
        }

        [HttpGet("{TravelId}")]
        public async Task<IActionResult> GetTravel(int? TravelId)
        {
            if (TravelId == null)
            {
                return BadRequest(new { message = "TravelId not Found !" });
            }
            int Id = (int)TravelId;
            TravelResponseDto response = await _service.GetTravelByIdAsync(Id);
            return Ok(response);
        }

        [HttpGet("{TravelId}/travelers")]
        public async Task<IActionResult> GetTravelerByTravelId(int? TravelId)
        {
            if (TravelId == null)
            {
                return BadRequest(new { message = "TravelId not Found !" });
            }
            int Id = (int)TravelId;
            TravelWithTravelerResponseDto response = await _service.GetTravelersByTravelId(Id);
            return Ok(response);
        }

        [HttpGet("hr")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetTravels(int PageSize = 10, int PageNumber = 1)
        {
            if (PageNumber <= 0 || PageSize <= 0)
                throw new InvalidOperationCustomException($"{nameof(PageNumber)} and {nameof(PageSize)} size must be greater than 0.");
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<TravelResponseDto> response = await _service.GetHrCreatedTravels(CurrentUserId, PageSize, PageNumber);
            return Ok(response);
        }

        [HttpDelete("{TravelId}")]
        public async Task<IActionResult> DeleteTravel(int? TravelId)
        {
            if (TravelId == null)
            {
                return BadRequest(new { message = "TravelId not Found !" });
            }
            int Id = (int)TravelId;
            await _service.RemoveTravel(Id);
            return Ok($"Travel with Id : {Id} deleted Successfully !");
        }

        [HttpPost("{TravelId}/document")]
        public async Task<IActionResult> AddDocument(int? TravelId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File Not Found !" });
            Console.WriteLine(file.FileName);
            return Ok(new { message = "Geted !" });
        }

        [HttpPost("{TravelId}/expense")]
        public async Task<IActionResult> AddExpense(
            int? TravelId, 
            ExpenseCreateDto dto
            //,List<IFormFile> files
            )
        {
            List<IFormFile> files = new List<IFormFile>();
            if (TravelId == null || dto == null 
                //|| files == null || files.Count == 0
                )
            {
                return BadRequest("Required Resource Not Found !");
            }
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            int Id = (int)TravelId;
            ExpenseResponseDto respone = await _service.AddExpense(Id, CurrentUserId, dto,files);
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
