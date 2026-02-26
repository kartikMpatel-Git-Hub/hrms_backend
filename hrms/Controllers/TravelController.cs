using hrms.CustomException;
using hrms.Dto.Request.Travel;
using hrms.Dto.Request.Travel.Document;
using hrms.Dto.Response.Expense;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Travel;
using hrms.Dto.Response.Travel.Document;
using hrms.Dto.Response.Traveler;
using hrms.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace hrms.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    [EnableCors("MyAllowSpecificOrigins")]
    public class TravelController : Controller
    {
        private readonly ITravelService _service;
        private readonly ICloudinaryService _cloudinary;
        private readonly ILogger<TravelController> _logger;

        public TravelController(ITravelService service, ICloudinaryService cloudinary, ILogger<TravelController> logger)
        {
            _service = service;
            _cloudinary = cloudinary;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> CreateTravel(TravelCreateDto dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            TravelResponseDto response = await _service.CreateTravelAsync(dto, CurrentUserId);
            _logger.LogInformation("[{Method}] {Url} - Travel created by HR {UserId} successfully", Request.Method, Request.Path, CurrentUserId);
            return Ok(response);
        }

        [HttpPost("{TravelId}/travelers")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> AddTravelers(int? TravelId, TravelerAddDto Dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (TravelId == null || Dto == null)
            {
                throw new NotFoundCustomException($"Travel Id or Request Body Not Found !");
            }
            int Id = (int)TravelId;
            await _service.AddTraveler(Id, Dto);
            _logger.LogInformation("[{Method}] {Url} - Travelers added to travel {TravelId} successfully", Request.Method, Request.Path, TravelId);
            return Ok("Traveler's Added Succesfully !");
        }

        [HttpPost("{TravelId}/travelers/{TravelerId}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> AddTraveler(int? TravelId,int? TravelerId )
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (TravelId == null || TravelerId == null)
            {
                throw new NotFoundCustomException($"Travel Id or Traveler Id Not Found !");
            }
            int travelId = (int)TravelId;
            int travelerId = (int)TravelerId;
            TravelerDto  response = await _service
                .AddTraveler(travelId,travelerId);
            _logger.LogInformation("[{Method}] {Url} - Traveler {TravelerId} added to travel {TravelId} successfully", Request.Method, Request.Path, TravelerId, TravelId);
            return Ok(response);
        }

        [HttpPut("{TravelId}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> UpdateTravel(int? TravelId, TravelUpdateDto dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (TravelId == null ||  dto == null)
            {
                return BadRequest(new {message = "TravelId or Updated field not Found !"});
            }
            int Id = (int)TravelId;
            TravelResponseDto response = await _service.UpdateTravelById(Id, dto);
            _logger.LogInformation("[{Method}] {Url} - Travel {TravelId} updated successfully", Request.Method, Request.Path, TravelId);
            return Ok(response);
        }

        [HttpGet("{TravelId}")]
        public async Task<IActionResult> GetTravel(int? TravelId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (TravelId == null)
            {
                return BadRequest(new { message = "TravelId not Found !" });
            }
            int Id = (int)TravelId;
            TravelResponseDto response = await _service.GetTravelByIdAsync(Id);
            _logger.LogInformation("[{Method}] {Url} - Fetched travel {TravelId} successfully", Request.Method, Request.Path, TravelId);
            return Ok(response);
        }

        [HttpGet("{TravelId}/travelers")]
        public async Task<IActionResult> GetTravelerByTravelId(int? TravelId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (TravelId == null)
            {
                return BadRequest(new { message = "TravelId not Found !" });
            }
            int Id = (int)TravelId;
            TravelWithTravelerResponseDto response = await _service.GetTravelersByTravelId(Id);
            _logger.LogInformation("[{Method}] {Url} - Fetched travelers for travel {TravelId} successfully", Request.Method, Request.Path, TravelId);
            return Ok(response);
        }

        [HttpGet("hr")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetTravels(int PageSize = 10, int PageNumber = 1)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (PageNumber <= 0 || PageSize <= 0)
                throw new InvalidOperationCustomException($"{nameof(PageNumber)} and {nameof(PageSize)} size must be greater than 0.");
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<TravelResponseDto> response = await _service.GetHrCreatedTravels(CurrentUserId, PageSize, PageNumber);
            _logger.LogInformation("[{Method}] {Url} - Fetched travels for HR {UserId} successfully", Request.Method, Request.Path, CurrentUserId);
            return Ok(response);
        }

        [HttpDelete("{TravelId}")]
        public async Task<IActionResult> DeleteTravel(int? TravelId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (TravelId == null)
            {
                return BadRequest(new { message = "TravelId not Found !" });
            }
            int Id = (int)TravelId;
            await _service.RemoveTravel(Id);
            _logger.LogInformation("[{Method}] {Url} - Travel {TravelId} deleted successfully", Request.Method, Request.Path, TravelId);
            return Ok($"Travel with Id : {Id} deleted Successfully !");
        }

        [Authorize(Roles = "HR,EMPLOYEE")]
        [HttpPost("{TravelId}/traveler/{TravelerId}/document")]
        public async Task<IActionResult> AddDocument(
            int? TravelId,
            int? TravelerId,
            TravelDocumentCreateDto dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (dto.Document == null)
                return BadRequest(new { message = "Document Not Found !" });
            if (TravelId == null)
                return BadRequest(new { message = "Travel Id Not Found !" });
            if (TravelerId == null)
                return BadRequest(new { message = "Traveler Id Not Found !" });
            int travelId = (int)TravelId;
            int travelerId = (int)TravelerId;
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            TravelDocumentResponseDto response = await _service.AddTravelDocument(travelId, travelerId, CurrentUserId, dto);
            _logger.LogInformation("[{Method}] {Url} - Document added to travel {TravelId} for traveler {TravelerId} successfully", Request.Method, Request.Path, TravelId, TravelerId);
            return Ok(response);
        }


        [HttpGet("{TravelId}/traveler/{TravelerId}/document")]
        public async Task<IActionResult> GetTravelTravelerDocument(
            int? TravelId,
            int? TravelerId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (TravelId == null)
                return BadRequest(new { message = "Travel Id Not Found !" });
            if (TravelerId == null)
                return BadRequest(new { message = "Traveler Id Not Found !" });
            int travelId = (int)TravelId;
            int travelerId = (int)TravelerId;
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            List<TravelDocumentResponseDto> response = await _service.GetTravelDocument(travelId, travelerId,CurrentUserId);
            _logger.LogInformation("[{Method}] {Url} - Fetched {Count} documents for travel {TravelId} and traveler {TravelerId} successfully", Request.Method, Request.Path, response.Count, TravelId, TravelerId);
            return Ok(response);
        }

        [HttpGet("employee")]
        [Authorize(Roles = "EMPLOYEE")]
        public async Task<IActionResult> GetMyTravelsForEmployee(int PageSize = 10, int PageNumber = 1)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (PageNumber <= 0 || PageSize <= 0)
                throw new InvalidOperationCustomException($"{nameof(PageNumber)} and {nameof(PageSize)} size must be greater than 0.");
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<TravelResponseDto> response = await _service.GetEmployeeTravels(CurrentUserId, PageSize, PageNumber);
            _logger.LogInformation("[{Method}] {Url} - Fetched travels for employee {UserId} successfully", Request.Method, Request.Path, CurrentUserId);
            return Ok(response);
        }

        [HttpGet("traveler/{travelerId}")]
        public async Task<IActionResult> GetTravelsByTravelerId(int? travelerId, int PageSize = 10, int PageNumber = 1)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (travelerId == null)
                return BadRequest(new { message = "Traveler Id Not Found !" });
            if (PageNumber <= 0 || PageSize <= 0)
                throw new InvalidOperationCustomException($"{nameof(PageNumber)} and {nameof(PageSize)} size must be greater than 0.");
            int TravelerId = (int)travelerId;
            PagedReponseDto<TravelResponseDto> response = await _service.GetTravelsByTravelerId(TravelerId, PageSize, PageNumber);
            _logger.LogInformation("[{Method}] {Url} - Fetched travels for traveler {TravelerId} successfully", Request.Method, Request.Path, travelerId);
            return Ok(response);
        }

        [HttpGet("{travelId}/traveler/{travelerId}/expenses")]
        public async Task<IActionResult> GetExpensesByTravelIdAndTravelerId(int? travelId, int? travelerId, int PageSize = 10, int PageNumber = 1)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (travelId == null)
                return BadRequest(new { message = "Travel Id Not Found !" });
            if (travelerId == null)
                return BadRequest(new { message = "Traveler Id Not Found !" });
            if (PageNumber <= 0 || PageSize <= 0)
                throw new InvalidOperationCustomException($"{nameof(PageNumber)} and {nameof(PageSize)} size must be greater than 0.");
            int TravelId = (int)travelId;
            int TravelerId = (int)travelerId;
            PagedReponseDto<ExpenseResponseDto> response = await _service.GetExpensesByTravelIdAndTravelerId(TravelId, TravelerId, PageSize, PageNumber);
            _logger.LogInformation("[{Method}] {Url} - Fetched expenses for travel {TravelId} and traveler {TravelerId} successfully", Request.Method, Request.Path, travelId, travelerId);
            return Ok(response);
        }

        [HttpGet("{travelId}/traveler/{travelerId}/documents")]
        public async Task<IActionResult> GetDocumentsByTravelIdAndTravelerId(int? travelId, int? travelerId, int PageSize = 10, int PageNumber = 1)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (travelId == null)
                return BadRequest(new { message = "Travel Id Not Found !" });
            if (travelerId == null)
                return BadRequest(new { message = "Traveler Id Not Found !" });
            if (PageNumber <= 0 || PageSize <= 0)
                throw new InvalidOperationCustomException($"{nameof(PageNumber)} and {nameof(PageSize)} size must be greater than 0.");
            int TravelId = (int)travelId;
            int TravelerId = (int)travelerId;
            PagedReponseDto<TravelDocumentResponseDto> response = await _service.GetDocumentsByTravelIdAndTravelerId(TravelId, TravelerId, PageSize, PageNumber);
            _logger.LogInformation("[{Method}] {Url} - Fetched documents for travel {TravelId} and traveler {TravelerId} successfully", Request.Method, Request.Path, travelId, travelerId);
            return Ok(response);
        }

    }
}
