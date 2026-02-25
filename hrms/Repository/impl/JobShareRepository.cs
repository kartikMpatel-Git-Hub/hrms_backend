using hrms.Data;
using hrms.Dto.Response.Other;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

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

        public async Task<PagedReponseOffSet<JobShared>> GetSharedJobByJobId(int jobId, int pageNumber, int pageSize)
        {
            var total = await _db.SharedJobs
                            .Where(js => js.JobId == jobId && !js.is_deleted)
                            .CountAsync();
            var data = await _db.SharedJobs
                    .Where(js => js.JobId == jobId && !js.is_deleted)
                    .Include(j => j.Shared)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(js => js.SharedAt)
                    .ToListAsync();
            return new PagedReponseOffSet<JobShared>(data, total, pageNumber, pageSize);
        }
    }
}
