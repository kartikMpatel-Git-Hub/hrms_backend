using hrms.Dto.Response.Notification;
using hrms.Dto.Response.Other;

namespace hrms.Service
{
    public interface INotificationService
    {
        Task<PagedReponseDto<NotificationResponseDto>> GetMyNotification(int userId, int pageNumber, int pageSize);
        Task<int> GetMyNotificationCount(int userId);
    }
}
