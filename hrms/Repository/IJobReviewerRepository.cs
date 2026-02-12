using hrms.Model;

namespace hrms.Repository
{
    public interface IJobReviewerRepository
    {
        Task<JobReviewer> AddReviewer(JobReviewer reviewer);
    }
}
