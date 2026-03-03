using hrms.Data;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Repository.impl
{
    public class SystemSettingsRepository(ApplicationDbContext _db) : ISystemSettingsRepository
    {
        public async Task<SystemSettings> GetSystemSettingsAsync()
        {
            var settings = await _db.SystemSettings
                .Include(s => s.DefaultHr)
                .Include(s => s.UpdatedBy)
                .FirstOrDefaultAsync();
            if(settings == null)
            {
                var s = new SystemSettings
                {
                    BirthdayImageUrl = "https://res.cloudinary.com/dcpvyecl2/image/upload/v1771783750/uploads/fwm4hoo23cse8kkwahbc.jpg",
                    AnniversaryImageUrl = "https://res.cloudinary.com/dcpvyecl2/image/upload/v1771779505/uploads/mvieaeol0gzhhys440hn.jpg",
                    DefaultProfileImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRCS2wb0ixNEu-qFWrF9k1ml03x2jJ6Fc_eKA&s",
                    UpdatedAt = DateTime.Now
                };
                var res = await _db.SystemSettings.AddAsync(s);
                await _db.SaveChangesAsync();
                return res.Entity;
            }
            return settings;
        }

        public async Task<SystemSettings> UpdateDefaultHrAsync(int defaultHrId, int updatedById)
        {
            var settings = await GetSystemSettingsAsync();
            settings.DefaultHrId = defaultHrId;
            settings.UpdatedById = updatedById;
            settings.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return await GetSystemSettingsAsync();
        }

        public async Task<SystemSettings> UpdateBirthdayImageAsync(string imageUrl, int updatedById)
        {
            var settings = await GetSystemSettingsAsync();
            settings.BirthdayImageUrl = imageUrl;
            settings.UpdatedById = updatedById;
            settings.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return await GetSystemSettingsAsync();
        }

        public async Task<SystemSettings> UpdateAnniversaryImageAsync(string imageUrl, int updatedById)
        {
            var settings = await GetSystemSettingsAsync();
            settings.AnniversaryImageUrl = imageUrl;
            settings.UpdatedById = updatedById;
            settings.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return await GetSystemSettingsAsync();
        }

        public async Task<SystemSettings> UpdateDefaultProfileImageAsync(string imageUrl, int updatedById)
        {
            var settings = await GetSystemSettingsAsync();
            settings.DefaultProfileImageUrl = imageUrl;
            settings.UpdatedById = updatedById;
            settings.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return await GetSystemSettingsAsync();
        }

    }
}