using hrms.CustomException;
using hrms.Dto.Request.Job;
using hrms.Dto.Request.Referral;
using hrms.Dto.Request.Share;
using hrms.Dto.Response.Job;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Referred;
using hrms.Dto.Response.Share;
using hrms.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace hrms.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class JobController : Controller
    {
        private readonly IJobService _service;

        public JobController(IJobService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateJob(JobCreateDto? job)
        {
            if (job == null)
                throw new ArgumentNullException("Job Body Not Found !");

            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int hrId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            JobResponseDto response = await _service.CreateJob(hrId,job);
            return Ok(response);
        }


        [HttpPut("{jobId}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> UpdateJob(int? jobId, JobUpdateDto? job)
        {
            if (jobId == null || job == null)
                throw new ArgumentNullException("Job Id or Job Body Not Found !");
            var CurrentUser = User;
            if (CurrentUser == null)               
                throw new UnauthorizedCustomException($"Unauthorized Access !"); 
            int hrId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            JobResponseDto response = await _service.UpdateJob((int)jobId, hrId, job);
            return Ok(response);
        }

        [HttpGet("hr/created")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetJobsCreatedByHr(int pageNumber = 1, int pageSize = 10)
        {
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int hrId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<JobResponseDto> jobs = await _service.GetJobsCreatedByHr(hrId, pageNumber, pageSize);

            return Ok(jobs);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllJobs(int pageNumber = 1, int pageSize = 10)
        {
            PagedReponseDto<JobResponseDto> jobs = await _service.GetAllJobs(pageNumber, pageSize);

            return Ok(jobs);
        }


        [HttpGet("{jobId}/reviewers")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetJobsReviewers(
            int? jobId)
        {
            if (jobId == null)
                throw new ArgumentNullException($"Job Id not found !");

            JobResponseWithReviewersDto reviewers = await _service.GetJobDetail((int)jobId);

            return Ok(reviewers);
        }

        [HttpGet("{jobId}")]
        public async Task<IActionResult> GetJobById(
            int? jobId)
        {
            if (jobId == null)
                throw new ArgumentNullException($"Job Id not found !");

            JobResponseDto reviewers = await _service.GetJobById((int)jobId);

            return Ok(reviewers);
        }

        [HttpPost("{jobId}/referre")]
        [Authorize(Roles = "EMPLOYEE,MANAGER,HR")]
        public async Task<IActionResult> ReferedJob(
            int? jobId,ReferralCreateDto? dto)
        {
            if (jobId == null || dto == null)
                throw new ArgumentNullException($"Job Id or Referal data not found !");

            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int referedBy = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            ReferralResponseDto response = await _service.CreateJobReferral((int)jobId, referedBy,(ReferralCreateDto)dto);

            return Ok(response);
        }

        [HttpGet("{jobId}/referred")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetReferredJobByJob(int? jobId,int pageNumber = 1,int pageSize = 10)
        {
            if (jobId == null)
                throw new ArgumentNullException($"Job Id not found !");
            if (pageNumber < 1 || pageSize < 1)
                throw new InvalidOperationCustomException($"Invalid Pagination !");
            PagedReponseDto<ReferralResponseDto> reponse
                = await _service.GetJobRefferalsByJobId((int)jobId,pageNumber,pageSize);
            return Ok(reponse);
        }

        [HttpGet("{jobId}/referred/{refId}")]
        public async Task<IActionResult> GetReferredJobByJobAndRef(int? jobId,int? refId)
        {
            if (jobId == null || refId == null)
                throw new ArgumentNullException($"Job Id or Referral Id not found !");
            ReferralResponseDto reponse
                = await _service.GetJobRefferalsByRefId((int)jobId,(int)refId);
            return Ok(reponse);
        }

        [HttpGet("referred/hr")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetReferredJobByHr(int pageNumber = 1, int pageSize = 10)
        {
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            if (pageNumber < 1 || pageSize < 1)
                throw new InvalidOperationCustomException($"Invalid Pagination !");

            int hrId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<ReferralResponseDto> reponse
                = await _service.GetJobRefferalsByHrId((int)hrId, pageNumber, pageSize);
            return Ok(reponse);
        }


        [HttpGet("referred/employee")]
        [Authorize(Roles = "EMPLOYEE")]
        public async Task<IActionResult> GetReferredJobByEmployee(int pageNumber = 1, int pageSize = 10)
        {
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            if (pageNumber < 1 || pageSize < 1)
                throw new InvalidOperationCustomException($"Invalid Pagination !");

            int employeeId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<ReferralResponseDto> reponse
                = await _service.GetJobRefferalsByEmployeeId((int)employeeId, pageNumber, pageSize);
            return Ok(reponse);
        }

        [HttpPost("{jobId}/share")]
        [Authorize(Roles = "EMPLOYEE,MANAGER,HR")]
        public async Task<IActionResult> ShareJob(
            int? jobId, ShareCreateDto? dto)
        {
            if (jobId == null || dto == null)
                throw new ArgumentNullException($"Job Id or shared to email not found !");

            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int referedBy = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            SharedJobResponseDto response = await _service.CreateJobShare((int)jobId, referedBy, (ShareCreateDto)dto);

            return Ok(response);
        }

        [HttpGet("{jobId}/share")]
        public async Task<IActionResult> GetSharedJobByJobId(int? jobId, int pageNumber = 1, int pageSize = 10)
        {
            if (jobId == null)
                throw new ArgumentNullException($"Job Id not found !");
            if (pageNumber < 1 || pageSize < 1)
                throw new InvalidOperationCustomException($"Invalid Pagination !");

            PagedReponseDto<SharedJobResponseDto> response = await _service.GetSharedJobByJobId((int)jobId,pageNumber,pageSize);

            return Ok(response);
        }

        [HttpDelete("{jobId}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> DeleteJob(int? jobId)
        {
            if (jobId == null)
                throw new ArgumentNullException($"Job Id not found !");

            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int hrId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            await _service.DeleteJob((int)jobId, hrId);
            return Ok(new { message = "Job Deleted Successfully !" });
        }
    }
}
