using hrms.CustomException;
using hrms.Dto.Response.Notification;
using hrms.Dto.Response.Other;
using hrms.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace hrms.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            this._notificationService = notificationService;   
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotification(int? pageNumber = 1, int? pageSize = 10)
        {
            if (pageNumber == null || pageSize == null || pageNumber <= 0 || pageSize <= 0)
            {
                throw new ArgumentNullException("Pagenumber and Pagesize not found !");
            }
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int userId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<NotificationResponseDto> notifications =
                await _notificationService.GetMyNotification(userId, (int)pageNumber, (int)pageSize);
            return Ok(notifications);
        }
        [HttpGet("count")]
        public async Task<IActionResult> GetMyNotificationCount()
        {
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int userId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            int count = await _notificationService.GetMyNotificationCount(userId);
            return Ok(new {count});
        }
    }
}
