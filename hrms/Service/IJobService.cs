using hrms.Dto.Request.Job;
using hrms.Dto.Request.Referral;
using hrms.Dto.Request.Share;
using hrms.Dto.Response.Job;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Referred;
using hrms.Dto.Response.Share;

namespace hrms.Service
{
    public interface IJobService
    {
        Task<JobResponseDto> CreateJob(int hrId, JobCreateDto job);
        Task<ReferralResponseDto> CreateJobReferral(int jobId, int referedBy, ReferralCreateDto dto);
        Task<SharedJobResponseDto> CreateJobShare(int jobId, int referedBy, ShareCreateDto dto);
        Task DeleteJob(int jobId, int hrId);
        Task<PagedReponseDto<JobResponseDto>> GetAllJobs(int pageNumber, int pageSize);
        Task<JobResponseDto> GetJobById(int jobId);
        Task<JobResponseWithReviewersDto> GetJobDetail(int jobId);
        Task<PagedReponseDto<ReferralResponseDto>> GetJobRefferalsByEmployeeId(int employeeId, int pageNumber, int pageSize);
        Task<PagedReponseDto<ReferralResponseDto>> GetJobRefferalsByHrId(int hrId, int pageNumber, int pageSize);
        Task<PagedReponseDto<ReferralResponseDto>> GetJobRefferalsByJobId(int jobId, int pageNumber, int pageSize);
        Task<ReferralResponseDto> GetJobRefferalsByRefId(int jobId, int refId);
        Task<PagedReponseDto<JobResponseDto>> GetJobsCreatedByHr(int hrId,int pageNumber,int pageSize);
        Task<PagedReponseDto<JobResponseDto>> GetJobToReview(int reviewerId, int pageNumber, int pageSize);
        Task<PagedReponseDto<SharedJobResponseDto>> GetSharedJobByJobId(int jobId, int pageNumber, int pageSize);
        Task<JobResponseDto> UpdateJob(int jobId, int hrId, JobUpdateDto job);
        Task<ReferralResponseDto> UpdateJobReferralStatus(int jobId, int refId, string status);
    }
}
