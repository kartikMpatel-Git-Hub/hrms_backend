using hrms.Dto.Response.Other;
using hrms.Model;

namespace hrms.Repository
{
    public interface INotificationRepository
    {
        Task<int> GetMyNotificationCount(int userId);
        Task<PagedReponseOffSet<Notification>> GetMyNotifications(int userId, int pageNumber, int pageSize);
    }
}
