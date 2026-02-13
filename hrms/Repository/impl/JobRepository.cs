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

        public JobRepository(ApplicationDbContext db)
        {
            this._db = db;
        }

        public async Task<Job> CreateJob(Job newJob)
        {
            var addedEntity = await _db.Jobs.AddAsync(newJob);
            await _db.SaveChangesAsync();
            return addedEntity.Entity;
        }

        public async Task<PagedReponseOffSet<Job>> GetAllJobs(int pageNumber, int pageSize)
        {
            List<Job> jobs = await _db.Jobs
                .Where(j => j.is_deleted == false && j.IsActive == true)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            int totalCount = await _db.Jobs
                .Where(j => j.is_deleted == false && j.IsActive == true)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .CountAsync();
            return new PagedReponseOffSet<Job>(jobs, pageNumber, pageSize, totalCount);
        }

        public async Task<Job> GetJobById(int jobId)
        {
            Job job = await _db.Jobs
                .Where(j => j.Id == jobId && j.is_deleted == false && j.IsActive == true)
                .Include(j => j.Reviewers)
                    .ThenInclude(jr => jr.Reviewer)
                .Include(j => j.Contact)
                .FirstOrDefaultAsync();
            if (job == null)
                throw new NotFoundCustomException($"Job With Id : {jobId} Not found !");
            return job;
        }

        public async Task<PagedReponseOffSet<Job>> GetJobsCreatedByHr(int hrId, int pageNumber, int pageSize)
        {
            List<Job> jobs = await _db.Jobs
                .Where(j => j.CreatedBy == hrId && j.is_deleted == false && j.IsActive == true)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            int totalCount = await _db.Jobs
                .Where(j => j.CreatedBy == hrId && j.is_deleted == false && j.IsActive == true)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .CountAsync();
            return new PagedReponseOffSet<Job>(jobs, pageNumber,pageSize,totalCount);
        }
    }
}
