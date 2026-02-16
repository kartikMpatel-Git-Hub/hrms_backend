using AutoMapper;
using hrms.Dto.Request.Job;
using hrms.Dto.Request.Referral;
using hrms.Dto.Request.Share;
using hrms.Dto.Response.Job;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Referred;
using hrms.Dto.Response.Share;
using hrms.Model;
using hrms.Repository;
using System.Text;

namespace hrms.Service.impl
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _repository;
        private readonly ICloudinaryService _cloudinary;
        private readonly IUserRepository _userRepository;
        private readonly IJobReviewerService _jobReviewerService;
        private readonly IJobReferralRepository _jobReferralRepository;
        private readonly IJobShareRepository _jobShareRepository;
        private readonly IEmailService _email;
        private readonly IMapper _mappers;

        public JobService(IJobRepository repository, ICloudinaryService cloudinary
            , IJobReviewerService jobReviewerService, IMapper mapper, IUserRepository userRepository, IJobReferralRepository jobReferralRepository, IEmailService email, IJobShareRepository jobShareRepository)
        {
            _repository = repository;
            _cloudinary = cloudinary;
            _mappers = mapper;
            _userRepository = userRepository;
            _jobReviewerService = jobReviewerService;
            _jobReferralRepository = jobReferralRepository;
            _email = email;
            _jobShareRepository = jobShareRepository;
        }
        public async Task<JobResponseDto> CreateJob(int hrId, JobCreateDto job)
        {
            User createdBy = await _userRepository.GetHrById(hrId);
            User contactTo = await _userRepository.GetHrById(job.ContactTo);

            Job newJob = new Job()
            {
                Title = job.Title,
                JobRole = job.JobRole,
                Place = job.Place,
                Requirements = job.Requirements,
                JdUrl = await _cloudinary.UploadAsync(job.Jd),
                IsActive = true,
                CreatedBy = createdBy.Id,
                Creater = createdBy,
                ContactTo = contactTo.Id,
                Contact = contactTo
            };

            Job createdJob = await _repository.CreateJob(newJob);
            await _jobReviewerService.AddReviewers(createdJob,job.Reviewer);
            return _mappers.Map<JobResponseDto>(createdJob);
        }

        public async Task<ReferralResponseDto> CreateJobReferral(int jobId, int referedBy, ReferralCreateDto dto)
        {
            Job job = await _repository.GetJobById(jobId);
            User user = await _userRepository.GetById(referedBy);
            JobReferral jobReferral = new JobReferral()
            {
                ReferedPersonName = dto.ReferedPersonName,
                ReferedPersonEmail = dto.ReferedPersonEmail,
                CvUrl = await _cloudinary.UploadAsync(dto.Cv),
                Note = dto.Note,
                ReferedBy = referedBy,
                Referer = user,
                JobId = jobId,
                Job = job,
                is_deleted = false,
                ReferedAt = DateTime.Now
            };
            JobReferral savedReferral = await _jobReferralRepository.AddReferral(jobReferral);
            await MailAfterReferred(job,savedReferral);
            return _mappers.Map<ReferralResponseDto>(jobReferral);
        }


        public async Task<PagedReponseDto<JobResponseDto>> GetAllJobs(int pageNumber, int pageSize)
        {
            PagedReponseOffSet<Job> jobs = await _repository.GetAllJobs(pageNumber, pageSize);
            return _mappers.Map<PagedReponseDto<JobResponseDto>>(jobs);
        }

        public async Task<JobResponseDto> GetJobById(int jobId)
        {
            Job job = await _repository.GetJobById(jobId);
            return _mappers.Map<JobResponseDto>(job);
        }

        public async Task<JobResponseWithReviewersDto> GetJobDetail(int jobId)
        {
            Job job = await _repository.GetJobById(jobId);
            return _mappers.Map<JobResponseWithReviewersDto>(job);  
        }

        public async Task<PagedReponseDto<ReferralResponseDto>> GetJobRefferalsByEmployeeId(int employeeId, int pageNumber, int pageSize)
        {
            PagedReponseOffSet<JobReferral> pagedReponse = await _jobReferralRepository.GetAllJobReferalByEmployeeId(employeeId, pageNumber, pageSize);
            return _mappers.Map<PagedReponseDto<ReferralResponseDto>>(pagedReponse);
        }

        public async Task<PagedReponseDto<ReferralResponseDto>> GetJobRefferalsByHrId(int hrId, int pageNumber, int pageSize)
        {
            PagedReponseOffSet<JobReferral> pagedReponse = await _jobReferralRepository.GetAllJobReferalByHrId(hrId, pageNumber, pageSize);
            return _mappers.Map<PagedReponseDto<ReferralResponseDto>>(pagedReponse);
        }

        public async Task<PagedReponseDto<ReferralResponseDto>> GetJobRefferalsByJobId(int jobId, int pageNumber, int pageSize)
        {
            PagedReponseOffSet<JobReferral> pagedReponse = await _jobReferralRepository.GetAllJobReferalByJobId(jobId, pageNumber, pageSize);
            return _mappers.Map<PagedReponseDto<ReferralResponseDto>>(pagedReponse);
        }

        public async Task<ReferralResponseDto> GetJobRefferalsByRefId(int jobId, int refId)
        {
            JobReferral referral = await _jobReferralRepository.GetReferralById(refId);
            return _mappers.Map<ReferralResponseDto>(referral);
        }

        public async Task<PagedReponseDto<JobResponseDto>> GetJobsCreatedByHr(int hrId,int pageNumber,int pageSize)
        {
            User hr = await _userRepository.GetHrById(hrId);
            PagedReponseOffSet<Job> jobs = await _repository.GetJobsCreatedByHr(hrId,pageNumber,pageSize);
            return _mappers.Map<PagedReponseDto<JobResponseDto>>(jobs);
        }

        public async Task<SharedJobResponseDto> CreateJobShare(int jobId, int sharedBy, ShareCreateDto dto)
        {
            Job job = await _repository.GetJobById(jobId);
            User user = await _userRepository.GetById(sharedBy);
            JobShared jobShared = new JobShared()
            {
                SharedTo = dto.SharedTo,
                JobId = jobId,
                Job = job,
                SharedBy = sharedBy,
                Shared = user,
                is_deleted = false,
                SharedAt = DateTime.Now
            };
            JobShared CreatedJobShare = await _jobShareRepository.CreateShare(jobShared);
            await MailAfterShared(jobShared);
            return _mappers.Map<SharedJobResponseDto>(CreatedJobShare);
        }


        private async Task MailAfterReferred(Job job, JobReferral savedReferral)
        {
            StringBuilder mailBodyBuilder = new StringBuilder();
            mailBodyBuilder.Append("<h1>New Referral</h1>");
            mailBodyBuilder.Append($"<h4>Job Id : {job.Id}</h4>");
            mailBodyBuilder.Append($"<h4>Job Title : {job.Title}</h4>");
            mailBodyBuilder.Append($"<h4>Referred By : {savedReferral.ReferedBy}</h4>");
            mailBodyBuilder.Append($"<h4>Referred By : {savedReferral.Referer.FullName}</h4>");
            mailBodyBuilder.Append("<hr/>");
            mailBodyBuilder.Append($"<h2>Referred Person Name : {savedReferral.ReferedPersonName}</h2>");
            mailBodyBuilder.Append($"<h2>Referred Person Email : {savedReferral.ReferedPersonEmail}</h2>");
            mailBodyBuilder.Append("<hr/>");
            mailBodyBuilder.Append($"<h3>View CV : {savedReferral.CvUrl}</h3>");

            await _email.SendEmailAsync(job.Contact.Email, $"New Job Referral for {job.Title}", mailBodyBuilder.ToString());

            foreach (var reviewer in job.Reviewers)
            {
                await _email.SendEmailAsync(reviewer.Reviewer.Email, $"New Referral for {job.Title}", mailBodyBuilder.ToString());
            }
        }

        private async Task MailAfterShared(JobShared jobShared)
        {
            StringBuilder mailBodyBuilder = new StringBuilder();
            mailBodyBuilder.Append("<h1>Job Opening</h1>");
            mailBodyBuilder.Append($"<h4>Job Title : {jobShared.Job.Title}</h4>");
            mailBodyBuilder.Append($"<h4>Job Detail" +
                $"<h5>Title : {jobShared.Job.Title}<h5>" +
                $"<h5>Job Role : {jobShared.Job.JobRole}<h5>" +
                $"<h5>Place : {jobShared.Job.Place}<h5>" +
                $"<h5>Requirements : {jobShared.Job.Requirements}<h5>" +
                $"</h4>");
            mailBodyBuilder.Append("<hr/>");
            mailBodyBuilder.Append($"<h3>View JD : {jobShared.Job.JdUrl}</h3>");

            await _email.SendEmailAsync(jobShared.SharedTo, $"Job Opening for {jobShared.Job.Title}", mailBodyBuilder.ToString());
        }
    }
}
