using AutoMapper;
using hrms.Dto.Response.DailyCelebration;
using hrms.Dto.Response.Game;
using hrms.Model;
using hrms.Repository;
using hrms.Utility;
using Microsoft.Extensions.Caching.Memory;

namespace hrms.Service.impl
{
    public class DailyCelebrationService(
        IDailyCelebrationRepository _repository,
        IPostRepository _postRepository,
        IGameRepository _gameRepository,
        IMapper _mapper,
        IMemoryCache _cache,
        ILogger<DailyCelebrationService> _logger
        ) : IDailyCelebrationService
    {
        public async Task AddDailyCelebration()
        {
            _logger.LogInformation("Starting Daily Celebration Job at {Time}", DateTime.Now);
            List<User> birthdayUsers = await _repository.GetBirthdayUsersForToday();
            _logger.LogDebug("Found {Count} birthday users", birthdayUsers.Count);
            List<User> anniversaryUsers = await _repository.GetWorkAnniversaryUsersForToday();
            _logger.LogDebug("Found {Count} anniversary users", anniversaryUsers.Count);
            User system = await _repository.GetSystemUser();

            foreach (var user in birthdayUsers)
            {
                bool exists = await _repository.IsCelebrationAlreadyAdded(user.Id, DateTime.Now, EventType.Birthday);
                if (exists)
                {
                    _logger.LogInformation("Skipping birthday celebration for user {UserId} because it already exists", user.Id);
                    continue;
                }
                DailyCelebration celebration = new DailyCelebration()
                {
                    UserId = user.Id,
                    EventDate = DateTime.Now,
                    EventType = EventType.Birthday
                };
                await _repository.AddDailyCelebration(celebration);
                _logger.LogInformation("Added birthday celebration for user {UserId}", user.Id);
                await createPostForCelebration(system, user, "Birthday");
                _logger.LogDebug("Created post for birthday user {UserId}", user.Id);
                var key = CacheVersionKey.For(CacheDomains.DashboardCelebrations);
                _cache.Set(key, _cache.Get<int>(key) + 1);
            }
            foreach (var user in anniversaryUsers)
            {
                bool exists = await _repository.IsCelebrationAlreadyAdded(user.Id, DateTime.Now, EventType.WorkAnniversary);
                if (exists)
                {
                    _logger.LogInformation("Skipping anniversary celebration for user {UserId} because it already exists", user.Id);
                    continue;
                }
                DailyCelebration celebration = new DailyCelebration()
                {
                    UserId = user.Id,
                    EventDate = DateTime.Now,
                    EventType = EventType.WorkAnniversary
                };
                await _repository.AddDailyCelebration(celebration);
                _logger.LogInformation("Added work anniversary celebration for user {UserId}", user.Id);
                await createPostForCelebration(system, user, "Work Anniversary");
                _logger.LogDebug("Created post for work anniversary user {UserId}", user.Id);
                var key = CacheVersionKey.For(CacheDomains.DashboardCelebrations);
                _cache.Set(key, _cache.Get<int>(key) + 1);
            }
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.PostDetails));
            _logger.LogInformation("Cache Updated for Daily Celebrations at {Time}", DateTime.Now);
        }

        private void IncrementCacheVersion(string v)
        {
            var key = CacheVersionKey.For(v);
            _cache.Set(key, _cache.Get<int>(key) + 1);
            _logger.LogDebug("Cache version incremented for {CacheKey}, new version: {Version}", v, _cache.Get<int>(key));
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
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.DashboardCelebrations));
            var key = $"DailyCelebrations:version:{version}";
            if (_cache.TryGetValue(key, out List<DailyCelebrationResponseDto> cachedCelebrations))
            {
                _logger.LogDebug("Cache hit for daily celebrations (version {Version})", version);
                return cachedCelebrations;
            }
            _logger.LogDebug("Cache miss for daily celebrations (version {Version}) - querying repository", version);
            List<DailyCelebration> celebrations = await _repository.GetDailyCelebrationsForToday();
            var result = _mapper.Map<List<DailyCelebrationResponseDto>>(celebrations);
            _cache.Set(key, result, TimeSpan.FromMinutes(5));
            _logger.LogInformation("Retrieved {Count} daily celebrations from repository", result.Count);
            return result;
        }

        public async Task<List<UpcomingBookingSlotResponseDto>> GetUpcomingBookingSlotsForToday()
        {
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.DashboardUpcomingBookings));
            var key = $"UpcomingBookingSlots:version:{version}";
            if (_cache.TryGetValue(key, out List<UpcomingBookingSlotResponseDto> cachedBookingSlots))
            {
                _logger.LogDebug("Cache hit for upcoming booking slots (version {Version})", version);
                return cachedBookingSlots;
            }
            _logger.LogDebug("Cache miss for upcoming booking slots (version {Version}) - querying repository", version);
            List<GameSlot> gameSlots = await _gameRepository.GetUpcomingBookingSlotsForToday();
            var result = _mapper.Map<List<UpcomingBookingSlotResponseDto>>(gameSlots);
            _cache.Set(key, result, TimeSpan.FromMinutes(5));
            _logger.LogInformation("Retrieved {Count} upcoming booking slots from repository", result.Count);
            return result;
        }
    }
}