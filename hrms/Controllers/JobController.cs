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
        private readonly ILogger<JobController> _logger;

        public JobController(IJobService service, ILogger<JobController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateJob(JobCreateDto? job)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (job == null)
                throw new ArgumentNullException("Job Body Not Found !");

            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int hrId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            JobResponseDto response = await _service.CreateJob(hrId, job);
            _logger.LogInformation("[{Method}] {Url} - Job created by HR {HrId} successfully", Request.Method, Request.Path, hrId);
            return Ok(response);
        }


        [HttpPut("{jobId}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> UpdateJob(int? jobId, JobUpdateDto? job)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (jobId == null || job == null)
                throw new ArgumentNullException("Job Id or Job Body Not Found !");
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int hrId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            JobResponseDto response = await _service.UpdateJob((int)jobId, hrId, job);
            _logger.LogInformation("[{Method}] {Url} - Job {JobId} updated by HR {HrId} successfully", Request.Method, Request.Path, jobId, hrId);
            return Ok(response);
        }

        [HttpGet("hr/created")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetJobsCreatedByHr(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            int hrId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<JobResponseDto> jobs = await _service.GetJobsCreatedByHr(hrId, pageNumber, pageSize);
            _logger.LogInformation("[{Method}] {Url} - Fetched jobs created by HR {HrId} successfully", Request.Method, Request.Path, hrId);
            return Ok(jobs);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllJobs(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            PagedReponseDto<JobResponseDto> jobs = await _service.GetAllJobs(pageNumber, pageSize);
            _logger.LogInformation("[{Method}] {Url} - Fetched all jobs successfully", Request.Method, Request.Path);
            return Ok(jobs);
        }

        [HttpGet("{jobId}/reviewers")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetJobsReviewers(
            int? jobId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (jobId == null)
                throw new ArgumentNullException($"Job Id not found !");

            JobResponseWithReviewersDto reviewers = await _service.GetJobDetail((int)jobId);
            _logger.LogInformation("[{Method}] {Url} - Fetched reviewers for job {JobId} successfully", Request.Method, Request.Path, jobId);
            return Ok(reviewers);
        }

        [HttpGet("{jobId}")]
        public async Task<IActionResult> GetJobById(
            int? jobId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (jobId == null)
                throw new ArgumentNullException($"Job Id not found !");

            JobResponseDto reviewers = await _service.GetJobById((int)jobId);
            _logger.LogInformation("[{Method}] {Url} - Fetched job {JobId} successfully", Request.Method, Request.Path, jobId);
            return Ok(reviewers);
        }

        [HttpPost("{jobId}/referre")]
        [Authorize(Roles = "EMPLOYEE,MANAGER,HR")]
        public async Task<IActionResult> ReferedJob(
            int? jobId, ReferralCreateDto? dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (jobId == null || dto == null)
                throw new ArgumentNullException($"Job Id or Referal data not found !");

            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int referedBy = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            ReferralResponseDto response = await _service.CreateJobReferral((int)jobId, referedBy, (ReferralCreateDto)dto);
            _logger.LogInformation("[{Method}] {Url} - Job {JobId} referred by user {UserId} successfully", Request.Method, Request.Path, jobId, referedBy);
            return Ok(response);
        }

        [HttpGet("{jobId}/referred")]
        // [Authorize(Roles = "HR,")]
        public async Task<IActionResult> GetReferredJobByJob(int? jobId, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (jobId == null)
                throw new ArgumentNullException($"Job Id not found !");
            if (pageNumber < 1 || pageSize < 1)
                throw new InvalidOperationCustomException($"Invalid Pagination !");
            PagedReponseDto<ReferralResponseDto> reponse
                = await _service.GetJobRefferalsByJobId((int)jobId, pageNumber, pageSize);
            _logger.LogInformation("[{Method}] {Url} - Fetched referrals for job {JobId} successfully", Request.Method, Request.Path, jobId);
            return Ok(reponse);
        }

        [HttpGet("{jobId}/referred/{refId}")]
        public async Task<IActionResult> GetReferredJobByJobAndRef(int? jobId, int? refId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (jobId == null || refId == null)
                throw new ArgumentNullException($"Job Id or Referral Id not found !");
            ReferralResponseDto reponse
                = await _service.GetJobRefferalsByRefId((int)jobId, (int)refId);
            _logger.LogInformation("[{Method}] {Url} - Fetched referral {RefId} for job {JobId} successfully", Request.Method, Request.Path, refId, jobId);
            return Ok(reponse);
        }

        [HttpPut("{jobId}/referred/{refId}")]
        // [Authorize(Roles = "HR")]
        public async Task<IActionResult> UpdateJobReferralStatus(int? jobId, int? refId, ReferralStatusChangeDto? dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (jobId == null || refId == null || dto == null)
                throw new ArgumentNullException($"Job Id or Referral Id or Status not found !");
            ReferralResponseDto reponse
                = await _service.UpdateJobReferralStatus((int)jobId, (int)refId, dto.Status);
            _logger.LogInformation("[{Method}] {Url} - Referral {RefId} status updated for job {JobId} successfully", Request.Method, Request.Path, refId, jobId);
            return Ok(reponse);
        }

        [HttpGet("referred/hr")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetReferredJobByHr(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            if (pageNumber < 1 || pageSize < 1)
                throw new InvalidOperationCustomException($"Invalid Pagination !");

            int hrId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<ReferralResponseDto> reponse
                = await _service.GetJobRefferalsByHrId((int)hrId, pageNumber, pageSize);
            _logger.LogInformation("[{Method}] {Url} - Fetched referrals for HR {HrId} successfully", Request.Method, Request.Path, hrId);
            return Ok(reponse);
        }


        [HttpGet("referred/employee")]
        [Authorize(Roles = "EMPLOYEE")]
        public async Task<IActionResult> GetReferredJobByEmployee(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");
            if (pageNumber < 1 || pageSize < 1)
                throw new InvalidOperationCustomException($"Invalid Pagination !");

            int employeeId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<ReferralResponseDto> reponse
                = await _service.GetJobRefferalsByEmployeeId((int)employeeId, pageNumber, pageSize);
            _logger.LogInformation("[{Method}] {Url} - Fetched referred jobs for employee {EmployeeId} successfully", Request.Method, Request.Path, employeeId);
            return Ok(reponse);
        }

        [HttpPost("{jobId}/share")]
        [Authorize(Roles = "EMPLOYEE,MANAGER,HR")]
        public async Task<IActionResult> ShareJob(
            int? jobId, ShareCreateDto? dto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (jobId == null || dto == null)
                throw new ArgumentNullException($"Job Id or shared to email not found !");

            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int referedBy = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            SharedJobResponseDto response = await _service.CreateJobShare((int)jobId, referedBy, (ShareCreateDto)dto);
            _logger.LogInformation("[{Method}] {Url} - Job {JobId} shared by user {UserId} successfully", Request.Method, Request.Path, jobId, referedBy);
            return Ok(response);
        }

        [HttpGet("{jobId}/share")]
        public async Task<IActionResult> GetSharedJobByJobId(int? jobId, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (jobId == null)
                throw new ArgumentNullException($"Job Id not found !");
            if (pageNumber < 1 || pageSize < 1)
                throw new InvalidOperationCustomException($"Invalid Pagination !");

            PagedReponseDto<SharedJobResponseDto> response = await _service.GetSharedJobByJobId((int)jobId, pageNumber, pageSize);
            _logger.LogInformation("[{Method}] {Url} - Fetched shared entries for job {JobId} successfully", Request.Method, Request.Path, jobId);
            return Ok(response);
        }

        [HttpDelete("{jobId}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> DeleteJob(int? jobId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (jobId == null)
                throw new ArgumentNullException($"Job Id not found !");

            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int hrId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            await _service.DeleteJob((int)jobId, hrId);
            _logger.LogInformation("[{Method}] {Url} - Job {JobId} deleted by HR {HrId} successfully", Request.Method, Request.Path, jobId, hrId);
            return Ok(new { message = "Job Deleted Successfully !" });
        }

        [HttpGet("review")]
        public async Task<IActionResult> GetJobToReview(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if (pageNumber < 1 || pageSize < 1)
                throw new InvalidOperationCustomException($"Invalid Pagination !");
            var CurrentUser = User;
            if (CurrentUser == null)
                throw new UnauthorizedCustomException($"Unauthorized Access !");

            int reviewerId = Int32.Parse(CurrentUser.FindFirst(ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<JobResponseDto> response = await _service.GetJobToReview(reviewerId, pageNumber, pageSize);
            _logger.LogInformation("[{Method}] {Url} - Fetched jobs to review for reviewer {ReviewerId} successfully", Request.Method, Request.Path, reviewerId);
            return Ok(response);
        }
    }
}
