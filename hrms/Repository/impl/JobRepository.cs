using hrms.CustomException;
using hrms.Data;
using hrms.Dto.Response.Other;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Repository.impl
{
    public class JobRepository : IJobRepository
    {
        public readonly ApplicationDbContext _db;
        private readonly ILogger<JobRepository> _logger;

        public JobRepository(ApplicationDbContext db, ILogger<JobRepository> logger)
        {
            this._db = db;
            _logger = logger;
        }

        public async Task<Job> CreateJob(Job newJob)
        {
            var addedEntity = await _db.Jobs.AddAsync(newJob);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created Job with Id {Id}", addedEntity.Entity.Id);
            return addedEntity.Entity;
        }

        public async Task DeleteJob(Job job)
        {
            job.is_deleted = true;
            _db.Jobs.Update(job);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Soft-deleted Job with Id {Id}", job.Id);
        }

        public async Task<PagedReponseOffSet<Job>> GetAllJobs(int pageNumber, int pageSize)
        {
            int totalCount = await _db.Jobs
                .Where(j => j.is_deleted == false && j.IsActive == true)
                .CountAsync();
                
            List<Job> jobs = await _db.Jobs
                .Where(j => j.is_deleted == false && j.IsActive == true)
                .OrderByDescending(j => j.created_at)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            _logger.LogInformation("Fetched {Count} active jobs, page {Page}", jobs.Count, pageNumber);
            return new PagedReponseOffSet<Job>(jobs, pageNumber, pageSize, totalCount);
        }

        public async Task<Job> GetJobById(int jobId)
        {
            Job job = await _db.Jobs
                .Where(j => j.Id == jobId && j.is_deleted == false)
                .Include(j => j.Reviewers)
                    .ThenInclude(jr => jr.Reviewer)
                .Include(j => j.Contact)
                .FirstOrDefaultAsync();
            if (job == null)
                throw new NotFoundCustomException($"Job With Id : {jobId} Not found !");
            _logger.LogInformation("Fetched Job with Id {Id}", jobId);
            return job;
        }

        public async Task<PagedReponseOffSet<Job>> GetJobsCreatedByHr(int hrId, int pageNumber, int pageSize)
        {
            int totalCount = await _db.Jobs
                .Where(j => j.CreatedBy == hrId && j.is_deleted == false)
                .CountAsync();
            List<Job> jobs = await _db.Jobs
                .Where(j => j.CreatedBy == hrId && j.is_deleted == false)
                .OrderByDescending(j => j.created_at)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedReponseOffSet<Job>(jobs, pageNumber, pageSize, totalCount);
        }

        public async Task<PagedReponseOffSet<Job>> GetJobToReview(int reviewerId, int pageNumber, int pageSize)
        {
            int totalCount = await _db.Jobs
                .Where(j => (j.Reviewers.Any(r => r.ReviewerId == reviewerId) || j.CreatedBy == reviewerId) && j.is_deleted == false)
                .CountAsync();
            List<Job> jobs = await _db.Jobs
                .Where(j => (j.Reviewers.Any(r => r.ReviewerId == reviewerId) || j.CreatedBy == reviewerId) && j.is_deleted == false)
                .OrderByDescending(j => j.created_at)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            _logger.LogInformation("Fetched {Count} jobs to review for ReviewerId {ReviewerId}, page {Page}", jobs.Count, reviewerId, pageNumber);
            return new PagedReponseOffSet<Job>(jobs, pageNumber, pageSize, totalCount);
        }

        public async Task<Job> UpdateJob(Job jobToUpdate)
        {
            var updatedEntity = _db.Jobs.Update(jobToUpdate);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Updated Job with Id {Id}", jobToUpdate.Id);
            return updatedEntity.Entity;
        }
    }
}
