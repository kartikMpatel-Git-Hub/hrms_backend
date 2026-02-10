using hrms.CustomException;
using hrms.Dto.Request.Travel;
using hrms.Dto.Request.Travel.Document;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Travel;
using hrms.Dto.Response.Travel.Document;
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

        public TravelController(ITravelService service, ICloudinaryService cloudinary)
        {
            _service = service;
            _cloudinary = cloudinary;
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

        [Authorize(Roles = "HR,EMPLOYEE")]
        [HttpPost("{TravelId}/traveler/{TravelerId}/document")]
        public async Task<IActionResult> AddDocument(
            int? TravelId,
            int? TravelerId,
            TravelDocumentCreateDto dto)
        {

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
            return Ok(response);
        }


        [HttpGet("{TravelId}/traveler/{TravelerId}/document")]
        public async Task<IActionResult> GetTravelTravelerDocument(
            int? TravelId,
            int? TravelerId)
        {
            if (TravelId == null)
                return BadRequest(new { message = "Travel Id Not Found !" });
            if (TravelerId == null)
                return BadRequest(new { message = "Traveler Id Not Found !" });
            int travelId = (int)TravelId;
            int travelerId = (int)TravelerId;
            List<TravelDocumentResponseDto> response = await _service.GetTravelDocument(travelId, travelerId);
            return Ok(response);
        }

        [HttpGet("employee")]
        [Authorize(Roles = "EMPLOYEE")]
        public async Task<IActionResult> GetMyTravelsForEmployee(int PageSize = 10, int PageNumber = 1)
        {
            Console.WriteLine("EMP");
            if (PageNumber <= 0 || PageSize <= 0)
                throw new InvalidOperationCustomException($"{nameof(PageNumber)} and {nameof(PageSize)} size must be greater than 0.");
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int CurrentUserId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<TravelResponseDto> response = await _service.GetEmployeeTravels(CurrentUserId, PageSize, PageNumber);
            return Ok(response);
        }

    }
}
