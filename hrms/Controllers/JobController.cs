using hrms.CustomException;
using hrms.Dto.Request.Job;
using hrms.Dto.Response.Job;
using hrms.Dto.Response.Other;
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

        [HttpGet]
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
        [HttpGet("{jobId}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetJobsReviewers(
            int? jobId)
        {
            if (jobId == null)
                throw new ArgumentNullException($"Job Id not found !");

            JobResponseWithReviewersDto reviewers = await _service.GetJobDetail((int)jobId);

            return Ok(reviewers);
        }
    }
}
