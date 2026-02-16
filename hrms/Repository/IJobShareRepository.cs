using hrms.Model;

namespace hrms.Repository
{
    public interface IJobShareRepository
    {
        Task<JobShared> CreateShare(JobShared jobShared);
    }
}
