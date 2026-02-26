using hrms.Data;
using hrms.Dto.Response.Other;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Repository.impl
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<NotificationRepository> _logger;

        public NotificationRepository(ApplicationDbContext db, ILogger<NotificationRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Notification> CreateNotification(Notification notification)
        {
            var result = await _db.Notifications.AddAsync(notification);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created Notification with Id {Id} for UserId {UserId}", result.Entity.Id, notification.NotifiedTo);
            return result.Entity;
        }

        public async Task<int> GetMyNotificationCount(int userId)
        {
            int TotalRecords = await _db.Notifications
                .Where(n => n.NotifiedTo == userId && n.IsViewed == false)
                .CountAsync();
            _logger.LogInformation("Unread notification count for UserId {UserId}: {Count}", userId, TotalRecords);
            return TotalRecords;
        }

        public async Task<PagedReponseOffSet<Notification>> GetMyNotifications(int userId, int pageNumber, int pageSize)
        {
            var TotalRecords = await _db.Notifications
                .Where(n => n.NotifiedTo == userId)
                .CountAsync();
            _logger.LogInformation("Fetching notifications for UserId {UserId}, total: {Total}, page {Page}", userId, TotalRecords, pageNumber);
            List<Notification> notifications = await _db.Notifications
                .Where(n => n.NotifiedTo == userId)
                .OrderByDescending(n => n.NotificationDate)
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
