using AutoMapper;
using hrms.Dto.Response.Notification;
using hrms.Dto.Response.Other;
using hrms.Model;
using hrms.Repository;

namespace hrms.Service.impl
{
    public class NotificationService : INotificationService
    {

        private readonly INotificationRepository _repository;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedReponseDto<NotificationResponseDto>> GetMyNotification(int userId,int pageNumber, int pageSize)
        {
            PagedReponseOffSet<Notification> notifications = await _repository.GetMyNotifications(userId,pageNumber,pageSize);
            return _mapper.Map<PagedReponseDto<NotificationResponseDto>>(notifications);
        }
        public async Task<int> GetMyNotificationCount(int userId)
        {
            int count = await _repository.GetMyNotificationCount(userId);
            return count;
        }
    }
}
