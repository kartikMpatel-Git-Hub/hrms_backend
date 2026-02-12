using hrms.Data;
using hrms.Dto.Response.Other;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Repository.impl
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _db;

        public NotificationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<int> GetMyNotificationCount(int userId)
        {
            int TotalRecords = await _db.Notifications
                .Where(n => n.NotifiedTo == userId && n.IsViewed == false)
                .CountAsync();
            return TotalRecords;
        }

        public async Task<PagedReponseOffSet<Notification>> GetMyNotifications(int userId, int pageNumber, int pageSize)
        {
            var TotalRecords = await _db.Notifications
                .Where(n => n.NotifiedTo == userId)
                .CountAsync();
            Console.WriteLine($"Total Notification : {TotalRecords}");
            List<Notification> notifications = await _db.Notifications
                .OrderByDescending(n => n.NotificationDate)
                .Where(n => n.NotifiedTo == userId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            foreach (var notification in notifications)
            {
                notification.IsViewed = true;
                _db.Notifications.Update(notification);
            }
            await _db.SaveChangesAsync();
            PagedReponseOffSet<Notification> Response = new PagedReponseOffSet<Notification>(notifications, pageNumber, pageSize, TotalRecords);
            return Response;
        }
    }
}
