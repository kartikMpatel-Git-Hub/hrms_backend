using hrms.Dto.Response.Other;
using hrms.Model;

namespace hrms.Repository
{
    public interface IJobRepository
    {
        Task<Job> CreateJob(Job newJob);
        Task DeleteJob(Job job);
        Task<PagedReponseOffSet<Job>> GetAllJobs(int pageNumber, int pageSize);
        Task<Job> GetJobById(int jobId);
        Task<PagedReponseOffSet<Job>> GetJobsCreatedByHr(int hrId, int pageNumber, int pageSize);
        Task<PagedReponseOffSet<Job>> GetJobToReview(int reviewerId, int pageNumber, int pageSize);
        Task<Job> UpdateJob(Job jobToUpdate);
    }
}
