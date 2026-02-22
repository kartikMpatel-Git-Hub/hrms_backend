using hrms.Data;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Repository.impl
{
    public class DailyCelebrationRepository(ApplicationDbContext _db) : IDailyCelebrationRepository
    {
        public async Task<DailyCelebration> AddDailyCelebration(DailyCelebration dailyCelebration)
        {
            var result = await _db.DailyCelebrations.AddAsync(dailyCelebration);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<List<User>> GetBirthdayUsersForToday()
        {
            DateTime today = DateTime.Now.Date;
            List<User> users = await _db.Users
                .Where(u => u.DateOfBirth.Date.Month == today.Month 
                && u.DateOfBirth.Date.Day == today.Day
                && u.is_deleted == false)
                .ToListAsync();
            return users;
        }

        public async Task<List<DailyCelebration>> GetDailyCelebrationsForToday()
        {
            List<DailyCelebration> celebrations = 
                        await _db.DailyCelebrations
                        .Where(dc => dc.EventDate.Date == DateTime.Now.Date)
                        .Include(dc => dc.User)
                        .ToListAsync();
            return celebrations;
        }

        public async Task<User> GetSystemUser()
        {
            User system = await _db.Users.Where(u => u.Email == "system@gmail.com").FirstOrDefaultAsync();
            if(system == null)
            {
                system = new User()
                {
                    FullName = "System",
                    Email = "system@gmail.com",
                    is_deleted = false
                };
                await _db.Users.AddAsync(system);
                await _db.SaveChangesAsync();
            }
            return system;
        }

        public async Task<List<User>> GetWorkAnniversaryUsersForToday()
        {
            DateTime today = DateTime.Now.Date;
            List<User> users = await _db.Users
                .Where(
                u => u.DateOfJoin.Date.Month == today.Month && 
                u.DateOfJoin.Date.Day == today.Day && 
                u.is_deleted == false)
                .ToListAsync();
            return users;
        }

        public async Task<bool> IsCelebrationAlreadyAdded(int id, DateTime now, EventType eventType)
        {
            bool exists = await _db.DailyCelebrations
                    .AnyAsync(dc => dc.UserId == id && dc.EventDate.Date == now.Date && dc.EventType == eventType);
            return exists;
        }
    }
}