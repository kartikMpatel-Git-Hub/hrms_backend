using hrms.Dto.Response.Other;
using hrms.Model;

namespace hrms.Repository
{
    public interface IJobReferralRepository
    {
        Task<JobReferral> AddReferral(JobReferral jobReferral);
        Task<JobReferral> GetReferralById(int referralId);
        Task<PagedReponseOffSet<JobReferral>> GetAllJobReferalByHrId(int hrId,int pageNumber,int pageSize);
        Task<PagedReponseOffSet<JobReferral>> GetAllJobReferalByJobId(int jobId,int pageNumber,int pageSize);
        Task<PagedReponseOffSet<JobReferral>> GetAllJobReferalByEmployeeId(int refId, int pageNumber, int pageSize);
    }
}
