using hrms.Data;
using hrms.Model;

namespace hrms.Repository.impl
{
    public class JobReviewerRepository : IJobReviewerRepository
    {
        public readonly ApplicationDbContext _db;

        public JobReviewerRepository(ApplicationDbContext db)
        {
            this._db = db;
        }
        public async Task<JobReviewer> AddReviewer(JobReviewer reviewer)
        {
            var addedEntity = await _db.AddAsync(reviewer);
            await _db.SaveChangesAsync();
            return addedEntity.Entity;
        }


    }
}
