using AutoMapper;
using hrms.Dto.Response.Notification;
using hrms.Dto.Response.Other;
using hrms.Model;
using hrms.Repository;
using hrms.Utility;
using Microsoft.Extensions.Caching.Memory;

namespace hrms.Service.impl
{
    public class NotificationService(INotificationRepository _repository, IMapper _mapper, IMemoryCache _cache, ILogger<NotificationService> _logger) : INotificationService
    {
        public async Task<PagedReponseDto<NotificationResponseDto>> GetMyNotification(int userId, int pageNumber, int pageSize)
        {
            // var version = _cache.Get<int>(CacheVersionKey.ForUserNotifications(userId));
            // var key = $"MyNotifications:UserId:{userId}:version:{version}:page:{pageNumber}:size:{pageSize}";
            // if (_cache.TryGetValue(key, out PagedReponseDto<NotificationResponseDto> cachedNotifications))
            // {
            //     _logger.LogDebug("Cache hit for notifications of user with Id {UserId} (version {Version})", userId, version);
            //     return cachedNotifications;
            // }
            PagedReponseOffSet<Notification> notifications = await _repository.GetMyNotifications(userId, pageNumber, pageSize);
            PagedReponseDto<NotificationResponseDto> response = _mapper.Map<PagedReponseDto<NotificationResponseDto>>(notifications);
            // _cache.Set(key, response, TimeSpan.FromMinutes(60));
            return response;
        }
        public async Task<int> GetMyNotificationCount(int userId)
        {
            // var version = _cache.Get<int>(CacheVersionKey.ForUserNotifications(userId));
            // var key = $"NotificationCount:UserId:{userId}:version:{version}";
            int count = await _repository.GetMyNotificationCount(userId);
            // _cache.Set(key, count, TimeSpan.FromMinutes(60));
            return count;
        }
    }
}
