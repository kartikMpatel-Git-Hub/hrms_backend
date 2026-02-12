using hrms.Dto.Request.Job;
using hrms.Dto.Response.Job;
using hrms.Dto.Response.Other;

namespace hrms.Service
{
    public interface IJobService
    {
        Task<JobResponseDto> CreateJob(int hrId, JobCreateDto job);
        Task<JobResponseWithReviewersDto> GetJobDetail(int jobId);
        Task<PagedReponseDto<JobResponseDto>> GetJobsCreatedByHr(int hrId,int pageNumber,int pageSize);
    }
}
