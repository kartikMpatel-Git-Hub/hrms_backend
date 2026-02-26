using hrms.CustomException;
using hrms.Dto.Response.Notification;
using hrms.Dto.Response.Other;
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
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
        {
            this._notificationService = notificationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotification(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new ArgumentNullException("Pagenumber and Pagesize not found !");
            }
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int userId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<NotificationResponseDto> notifications =
                await _notificationService.GetMyNotification(userId, (int)pageNumber, (int)pageSize);
            _logger.LogInformation("[{Method}] {Url} - Fetched notifications for user {UserId} successfully", Request.Method, Request.Path, userId);
            return Ok(notifications);
        }
        [HttpGet("count")]
        public async Task<IActionResult> GetMyNotificationCount()
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int userId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            int count = await _notificationService.GetMyNotificationCount(userId);
            _logger.LogInformation("[{Method}] {Url} - Fetched notification count {Count} for user {UserId} successfully", Request.Method, Request.Path, count, userId);
            return Ok(new {count});
        }
    }
}
