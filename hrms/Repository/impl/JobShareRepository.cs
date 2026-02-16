using hrms.Data;
using hrms.Model;

namespace hrms.Repository.impl
{
    public class JobShareRepository : IJobShareRepository
    {
        public readonly ApplicationDbContext _db;

        public JobShareRepository(ApplicationDbContext db)
        {
            this._db = db;
        }
        public async Task<JobShared> CreateShare(JobShared jobShared)
        {
            var addedEntity = await _db.SharedJobs.AddAsync(jobShared);
            await _db.SaveChangesAsync();
            return addedEntity.Entity;
        }
    }
}
