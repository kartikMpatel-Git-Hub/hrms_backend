using AutoMapper;
using hrms.Dto.Response.DailyCelebration;
using hrms.Dto.Response.Game;
using hrms.Model;
using hrms.Repository;

namespace hrms.Service.impl
{
    public class DailyCelebrationService(
        IDailyCelebrationRepository _repository, 
        IPostRepository _postRepository,
        IGameRepository _gameRepository,
        IMapper _mapper
        ) : IDailyCelebrationService
    {
        public async Task AddDailyCelebration()
        {
            System.Console.WriteLine("Adding Daily Celebrations...");
            List<User> birthdayUsers = await _repository.GetBirthdayUsersForToday(); 
            List<User> anniversaryUsers = await _repository.GetWorkAnniversaryUsersForToday(); 
            User system = await _repository.GetSystemUser(); 
            foreach (var user in birthdayUsers)
            {
                bool exists = await _repository.IsCelebrationAlreadyAdded(user.Id, DateTime.Now, EventType.Birthday);
                if (exists)
                {
                    continue; 
                }
                DailyCelebration celebration = new DailyCelebration()
                {
                    UserId = user.Id,
                    EventDate = DateTime.Now,
                    EventType = EventType.Birthday
                };
                await _repository.AddDailyCelebration(celebration);
                await createPostForCelebration(system, user, "Birthday");
            }
            foreach (var user in anniversaryUsers)
            {
                bool exists = await _repository.IsCelebrationAlreadyAdded(user.Id, DateTime.Now, EventType.WorkAnniversary);
                if (exists)                {
                    continue; 
                }
                DailyCelebration celebration = new DailyCelebration()
                {
                    UserId = user.Id,
                    EventDate = DateTime.Now,
                    EventType = EventType.WorkAnniversary
                };
                await _repository.AddDailyCelebration(celebration);
                await createPostForCelebration(system, user, "Work Anniversary");
            }
        }

        private async Task createPostForCelebration(User system, User user, string v)
        {
            Post post = new Post()
            {
                PostUrl = v == "Birthday" ? "https://res.cloudinary.com/dcpvyecl2/image/upload/v1771783750/uploads/fwm4hoo23cse8kkwahbc.jpg" : "https://res.cloudinary.com/dcpvyecl2/image/upload/v1771779505/uploads/mvieaeol0gzhhys440hn.jpg",
                Title = v == "Birthday" ? "Happy Birthday!" : "Work Anniversary Celebration!",
                Description = v == "Birthday" ? $"Wishing {user.FullName} a very Happy Birthday! May your day be filled with joy and laughter." : $"Congratulations to {user.FullName} on their Work Anniversary! Thank you for being an invaluable part of our team.",
                PostById = system.Id,
                created_at = DateTime.Now,
                IsPublic = true,
                is_deleted = false
            };
            await _postRepository.CreatePost(post);
        }

        public async Task<List<DailyCelebrationResponseDto>> GetDailyCelebrationsForToday()
        {
            List<DailyCelebration> celebrations = await _repository.GetDailyCelebrationsForToday();
            return _mapper.Map<List<DailyCelebrationResponseDto>>(celebrations);
        }

        public async Task<List<UpcomingBookingSlotResponseDto>> GetUpcomingBookingSlotsForToday()
        {
            List<GameSlot> gameSlots = await _gameRepository.GetUpcomingBookingSlotsForToday();
            return _mapper.Map<List<UpcomingBookingSlotResponseDto>>(gameSlots);
        }
    }
}