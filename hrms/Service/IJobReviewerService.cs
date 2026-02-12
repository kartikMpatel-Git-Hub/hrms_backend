using hrms.Model;

namespace hrms.Service
{
    public interface IJobReviewerService
    {
        Task AddReviewers(Job createdJob, List<int> reviewer);
    }
}
