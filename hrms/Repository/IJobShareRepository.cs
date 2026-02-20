using hrms.Dto.Response.Other;
using hrms.Model;

namespace hrms.Repository
{
    public interface IJobShareRepository
    {
        Task<JobShared> CreateShare(JobShared jobShared);
        Task<PagedReponseOffSet<JobShared>> GetSharedJobByJobId(int jobId, int pageNumber, int pageSize);
    }
}
