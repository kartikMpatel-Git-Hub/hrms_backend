using hrms.Data;
using hrms.Model;

namespace hrms.Repository.impl
{
    public class JobReviewerRepository : IJobReviewerRepository
    {
        public readonly ApplicationDbContext _db;
        private readonly ILogger<JobReviewerRepository> _logger;

        public JobReviewerRepository(ApplicationDbContext db, ILogger<JobReviewerRepository> logger)
        {
            this._db = db;
            _logger = logger;
        }
        public async Task<JobReviewer> AddReviewer(JobReviewer reviewer)
        {
            var addedEntity = await _db.AddAsync(reviewer);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Added JobReviewer with Id {Id}", addedEntity.Entity.Id);
            return addedEntity.Entity;
        }


    }
}
