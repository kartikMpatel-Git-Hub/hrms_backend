using hrms.Data;
using hrms.Dto.Response.Other;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Repository.impl
{
    public class JobShareRepository : IJobShareRepository
    {
        public readonly ApplicationDbContext _db;
        private readonly ILogger<JobShareRepository> _logger;

        public JobShareRepository(ApplicationDbContext db, ILogger<JobShareRepository> logger)
        {
            this._db = db;
            _logger = logger;
        }
        public async Task<JobShared> CreateShare(JobShared jobShared)
        {
            var addedEntity = await _db.SharedJobs.AddAsync(jobShared);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created JobShare with Id {Id} for JobId {JobId}", addedEntity.Entity.Id, jobShared.JobId);
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
                    .OrderByDescending(js => js.SharedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            _logger.LogInformation("Fetched {Count} shared jobs for JobId {JobId}, page {Page}", data.Count, jobId, pageNumber);
            return new PagedReponseOffSet<JobShared>(data, pageNumber, pageSize, total);
        }
    }
}
