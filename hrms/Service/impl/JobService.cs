using AutoMapper;
using hrms.CustomException;
using hrms.Dto.Request.Job;
using hrms.Dto.Request.Referral;
using hrms.Dto.Request.Share;
using hrms.Dto.Response.Job;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Referred;
using hrms.Dto.Response.Share;
using hrms.Model;
using hrms.Repository;
using hrms.Utility;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace hrms.Service.impl
{
    public class JobService(
        IJobRepository _repository,
        ICloudinaryService _cloudinary,
        IJobReviewerService _jobReviewerService,
        IMapper _mappers,
        IUserRepository _userRepository,
        IJobReferralRepository _jobReferralRepository,
        IEmailService _email,
        IJobShareRepository _jobShareRepository,
        INotificationRepository _notificationRepository,
        ILogger<JobService> _logger,
        IMemoryCache _cache
        ) : IJobService
    {
        public async Task<JobResponseDto> CreateJob(int hrId, JobCreateDto job)
        {
            User createdBy = await _userRepository.GetHrById(hrId);
            User contactTo = await _userRepository.GetHrById(job.ContactTo);
            _logger.LogInformation("Creating job with title {Title} by HR {HrId}", job.Title, hrId);

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
            await _jobReviewerService.AddReviewers(createdJob, job.Reviewer);
            await SendNotification(createdJob);
            _logger.LogInformation("Job with title {Title} created successfully with Id {JobId}", job.Title, createdJob.Id);
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.JobDetails));
            return _mappers.Map<JobResponseDto>(createdJob);
        }

        private async Task SendNotification(Job createdJob)
        {
            Notification notification = new Notification()
            {
                NotifiedTo = createdJob.ContactTo,
                Title = "New Job Created",
                Description = $"A new job with title {createdJob.Title} has been created. Please check the job details.",
                IsViewed = false,
                NotificationDate = DateTime.Now
            };
            await _notificationRepository.CreateNotification(notification);
            _logger.LogInformation("Notification sent to contact HR with Id {ContactTo} for job creation with Id {JobId}", createdJob.ContactTo, createdJob.Id);

            foreach (var reviewer in createdJob.Reviewers)
            {
                Notification notificationForReviewer = new Notification()
                {
                    NotifiedTo = reviewer.ReviewerId,
                    Title = "New Job Created",
                    Description = $"A new job with title {createdJob.Title} has been created. Please check the job details.",
                    IsViewed = false,
                    NotificationDate = DateTime.Now
                };
                await _notificationRepository.CreateNotification(notificationForReviewer);
                _logger.LogInformation("Notification sent to reviewer with Id {ReviewerId} for job creation with Id {JobId}", reviewer.ReviewerId, createdJob.Id);
            }
        }

        public async Task<ReferralResponseDto> CreateJobReferral(int jobId, int referedBy, ReferralCreateDto dto)
        {
            Job job = await _repository.GetJobById(jobId);
            User user = await _userRepository.GetById(referedBy);
            _logger.LogInformation("Creating job referral for job with Id {JobId} by employee with Id {EmployeeId}", jobId, referedBy);
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
            _logger.LogInformation("Job referral created successfully for job with Id {JobId} by employee with Id {EmployeeId}", jobId, referedBy);
            await MailAfterReferred(job, savedReferral);
            IncrementCacheVersion(CacheVersionKey.ForJobReferrals(jobId));
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.JobReferrals));
            return _mappers.Map<ReferralResponseDto>(jobReferral);
        }

        public async Task<PagedReponseDto<JobResponseDto>> GetAllJobs(int pageNumber, int pageSize)
        {
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.JobDetails));
            var key = $"Jobs:pageNumber:{pageNumber}:pageSize:{pageSize}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<JobResponseDto> cachedJobs))
            {
                _logger.LogDebug("Cache hit for jobs list (version {Version})", version);
                return cachedJobs;
            }
            PagedReponseOffSet<Job> jobs = await _repository.GetAllJobs(pageNumber, pageSize);
            PagedReponseDto<JobResponseDto> response = _mappers.Map<PagedReponseDto<JobResponseDto>>(jobs);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved jobs list from repository and cached with version {Version}", version);
            _logger.LogInformation("Total jobs retrieved: {Count}", response.Data.Count);
            return response;
        }

        public async Task<JobResponseDto> GetJobById(int jobId)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForJobDetails(jobId));
            var key = $"Job:{jobId}:version:{version}";
            if (_cache.TryGetValue(key, out JobResponseDto cachedJob))
            {
                _logger.LogDebug("Cache hit for job details (version {Version})", version);
                return cachedJob;
            }
            Job job = await _repository.GetJobById(jobId);
            var response = _mappers.Map<JobResponseDto>(job);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved job details for job with Id {JobId} from repository and cached with version {Version}", jobId, version);
            return response;
        }

        public async Task<JobResponseWithReviewersDto> GetJobDetail(int jobId)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForJobDetails(jobId));
            var key = $"JobDetail:{jobId}:version:{version}";
            if (_cache.TryGetValue(key, out JobResponseWithReviewersDto cachedJobDetail))
            {
                _logger.LogDebug("Cache hit for job detail with reviewers (version {Version})", version);
                return cachedJobDetail;
            }
            Job job = await _repository.GetJobById(jobId);
            var response = _mappers.Map<JobResponseWithReviewersDto>(job);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved job detail with reviewers for job with Id {JobId} from repository and cached with version {Version}", jobId, version);
            return response;
        }

        public async Task<PagedReponseDto<ReferralResponseDto>> GetJobRefferalsByEmployeeId(int employeeId, int pageNumber, int pageSize)
        {
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.JobReferrals));
            var key = $"JobReferrals:employeeId:{employeeId}:pageNumber:{pageNumber}:pageSize:{pageSize}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<ReferralResponseDto> cachedReferrals))
            {
                _logger.LogDebug("Cache hit for job referrals by employee (version {Version})", version);
                return cachedReferrals;
            }
            PagedReponseOffSet<JobReferral> pagedReponse = await _jobReferralRepository.GetAllJobReferalByEmployeeId(employeeId, pageNumber, pageSize);
            PagedReponseDto<ReferralResponseDto> response = _mappers.Map<PagedReponseDto<ReferralResponseDto>>(pagedReponse);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved job referrals by employee with Id {EmployeeId} from repository and cached with version {Version}", employeeId, version);
            _logger.LogInformation("Total job referrals retrieved for employee with Id {EmployeeId}: {Count}", employeeId, response.Data.Count);
            return response;
        }

        public async Task<PagedReponseDto<ReferralResponseDto>> GetJobRefferalsByHrId(int hrId, int pageNumber, int pageSize)
        {
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.JobReferrals));
            var key = $"JobReferrals:hrId:{hrId}:pageNumber:{pageNumber}:pageSize:{pageSize}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<ReferralResponseDto> cachedReferrals))
            {
                _logger.LogDebug("Cache hit for job referrals by HR (version {Version})", version);
                return cachedReferrals;
            }
            PagedReponseOffSet<JobReferral> pagedReponse = await _jobReferralRepository.GetAllJobReferalByHrId(hrId, pageNumber, pageSize);
            PagedReponseDto<ReferralResponseDto> response = _mappers.Map<PagedReponseDto<ReferralResponseDto>>(pagedReponse);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved job referrals by HR with Id {HrId} from repository and cached with version {Version}", hrId, version);
            _logger.LogInformation("Total job referrals retrieved for HR with Id {HrId}: {Count}", hrId, response.Data.Count);
            return response;
        }

        public async Task<PagedReponseDto<ReferralResponseDto>> GetJobRefferalsByJobId(int jobId, int pageNumber, int pageSize)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForJobReferrals(jobId));
            var key = $"JobReferrals:jobId:{jobId}:pageNumber:{pageNumber}:pageSize:{pageSize}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<ReferralResponseDto> cachedReferrals))
            {
                _logger.LogDebug("Cache hit for job referrals by job (version {Version})", version);
                return cachedReferrals;
            }
            PagedReponseOffSet<JobReferral> pagedReponse = await _jobReferralRepository.GetAllJobReferalByJobId(jobId, pageNumber, pageSize);
            PagedReponseDto<ReferralResponseDto> response = _mappers.Map<PagedReponseDto<ReferralResponseDto>>(pagedReponse);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved job referrals by job with Id {JobId} from repository and cached with version {Version}", jobId, version);
            _logger.LogInformation("Total job referrals retrieved for job with Id {JobId}: {Count}", jobId, response.Data.Count);
            return response;
        }

        public async Task<ReferralResponseDto> GetJobRefferalsByRefId(int jobId, int refId)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForJobReferrals(jobId));
            var key = $"JobReferral:refId:{refId}:version:{version}";
            if (_cache.TryGetValue(key, out ReferralResponseDto cachedReferral))
            {
                _logger.LogDebug("Cache hit for job referral details (version {Version})", version);
                return cachedReferral;
            }
            JobReferral referral = await _jobReferralRepository.GetReferralById(refId);
            ReferralResponseDto referralResponse = _mappers.Map<ReferralResponseDto>(referral);
            _cache.Set(key, referralResponse, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved job referral details for referral with Id {RefId} from repository and cached with version {Version}", refId, version);    
            return referralResponse;
        }

        public async Task<PagedReponseDto<JobResponseDto>> GetJobsCreatedByHr(int hrId, int pageNumber, int pageSize)
        {
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.JobDetails));
            var key = $"JobsByHr:hrId:{hrId}:pageNumber:{pageNumber}:pageSize:{pageSize}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<JobResponseDto> cachedJobs))
            {
                _logger.LogDebug("Cache hit for jobs created by HR (version {Version})", version);
                return cachedJobs;
            }
            User hr = await _userRepository.GetHrById(hrId);
            PagedReponseOffSet<Job> jobs = await _repository.GetJobsCreatedByHr(hrId, pageNumber, pageSize);
            PagedReponseDto<JobResponseDto> response = _mappers.Map<PagedReponseDto<JobResponseDto>>(jobs);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved jobs created by HR with Id {HrId} from repository and cached with version {Version}", hrId, version);
            _logger.LogInformation("Total jobs retrieved for HR with Id {HrId}: {Count}", hrId, response.Data.Count);
            return response;
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
            SharedJobResponseDto response = _mappers.Map<SharedJobResponseDto>(CreatedJobShare);
            IncrementCacheVersion(CacheVersionKey.ForJobShares(jobId));
            _logger.LogInformation("Created job share for job with Id {JobId} and shared to {SharedTo}", jobId, dto.SharedTo);
            return response;

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
            System.Console.WriteLine("Job Shared Mail Function Called");
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
            System.Console.WriteLine("Job Shared Mail Sent");
        }

        public async Task<PagedReponseDto<SharedJobResponseDto>> GetSharedJobByJobId(int jobId, int pageNumber, int pageSize)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForJobShares(jobId));
            var key = $"SharedJobs:jobId:{jobId}:pageNumber:{pageNumber}:pageSize:{pageSize}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<SharedJobResponseDto> cachedSharedJobs))
            {
                _logger.LogDebug("Cache hit for shared jobs by job (version {Version})", version);
                return cachedSharedJobs;
            }
            PagedReponseOffSet<JobShared> pagedReponse = await _jobShareRepository.GetSharedJobByJobId(jobId, pageNumber, pageSize);
            var response = _mappers.Map<PagedReponseDto<SharedJobResponseDto>>(pagedReponse);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved shared jobs by job with Id {JobId} from repository and cached with version {Version}", jobId, version);
            _logger.LogInformation("Total shared jobs retrieved for job with Id {JobId}: {Count}", jobId, response.Data.Count);
            return response;
        }

        public async Task<JobResponseDto> UpdateJob(int jobId, int hrId, JobUpdateDto job)
        {
            Job jobToUpdate = await _repository.GetJobById(jobId);
            if (jobToUpdate.CreatedBy != hrId)
                throw new UnauthorizedAccessException($"Unauthorized Access !");
            if (job.Title != null)
                jobToUpdate.Title = job.Title;
            if (job.JobRole != null)
                jobToUpdate.JobRole = job.JobRole;
            if (job.Place != null)
                jobToUpdate.Place = job.Place;
            if (job.Requirements != null)
                jobToUpdate.Requirements = job.Requirements;
            if (job.IsActive != jobToUpdate.IsActive)
                jobToUpdate.IsActive = job.IsActive;
            Job updatedJob = await _repository.UpdateJob(jobToUpdate);
            await _email.SendEmailAsync(jobToUpdate.Contact.Email, $"Job Updated for {jobToUpdate.Title}", $"Job with Title {jobToUpdate.Title} has been updated. Please check the job details.");
            foreach (var reviewer in jobToUpdate.Reviewers)
            {
                string email = reviewer.Reviewer.Email;
                await _email.SendEmailAsync(email, $"Job Updated for {jobToUpdate.Title}", $"Job with Title {jobToUpdate.Title} has been updated. Please check the job details.");
            }
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.JobDetails));
            IncrementCacheVersion(CacheVersionKey.ForJobDetails(jobId));
            _logger.LogInformation("Job with Id {JobId} updated successfully by HR with Id {HrId}", jobId, hrId);
            return _mappers.Map<JobResponseDto>(updatedJob);
        }

        public async Task DeleteJob(int jobId, int hrId)
        {
            Job job = await _repository.GetJobById(jobId);
            if (job.CreatedBy != hrId)
                throw new UnauthorizedAccessException($"Unauthorized Access !");
            await _repository.DeleteJob(job);
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.JobDetails));
            IncrementCacheVersion(CacheVersionKey.ForJobDetails(jobId));
            IncrementCacheVersion(CacheVersionKey.ForJobReferrals(jobId));
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.JobReferrals));
            IncrementCacheVersion(CacheVersionKey.ForJobShares(jobId));
            _logger.LogInformation("Job with Id {JobId} deleted successfully by HR with Id {HrId}", jobId, hrId);
        }

        public async Task<ReferralResponseDto> UpdateJobReferralStatus(int jobId, int refId, string status)
        {
            JobReferral referral = await _jobReferralRepository.GetReferralById(refId);
            if (referral.JobId != jobId)
                throw new InvalidOperationCustomException($"Referral with Id {refId} does not belong to Job with Id {jobId} !");
            referral.Status = GetReferralStatus(status.ToLower());
            JobReferral updatedReferral = await _jobReferralRepository.UpdateReferral(referral);
            IncrementCacheVersion(CacheVersionKey.ForJobReferrals(jobId));
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.JobReferrals));
            _logger.LogInformation("Job referral with Id {RefId} updated successfully for job with Id {JobId}", refId, jobId);
            return _mappers.Map<ReferralResponseDto>(updatedReferral);
        }

        private ReferralStatus GetReferralStatus(string status)
        {
            switch (status)
            {
                case "pending":
                    return ReferralStatus.Pending;
                case "hired":
                    return ReferralStatus.Hired;
                case "rejected":
                    return ReferralStatus.Rejected;
                case "interview":
                    return ReferralStatus.Interview;
                case "shortlisted":
                    return ReferralStatus.Shortlisted;
                default:
                    throw new NotFoundCustomException($"Referral status : {status} not found !");
            }
        }

        public async Task<PagedReponseDto<JobResponseDto>> GetJobToReview(int reviewerId, int pageNumber, int pageSize)
        {
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.JobDetails));
            var key = $"JobsToReview:reviewerId:{reviewerId}:pageNumber:{pageNumber}:pageSize:{pageSize}:version:{version}";
            if (_cache.TryGetValue(key, out PagedReponseDto<JobResponseDto> cachedJobsToReview))
            {
                _logger.LogDebug("Cache hit for jobs to review by reviewer (version {Version})", version);
                return cachedJobsToReview;
            }
            PagedReponseOffSet<Job> jobs = await _repository.GetJobToReview(reviewerId, pageNumber, pageSize);
            PagedReponseDto<JobResponseDto> response = _mappers.Map<PagedReponseDto<JobResponseDto>>(jobs);
            _cache.Set(key, response, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved jobs to review for reviewer with Id {ReviewerId} from repository and cached with version {Version}", reviewerId, version);
            _logger.LogInformation("Total jobs to review retrieved for reviewer with Id {ReviewerId}: {Count}", reviewerId, response.Data.Count);
            return response;
        }

        private void IncrementCacheVersion(string versionKey)
        {
            var current = _cache.Get<int>(versionKey);
            _cache.Set(versionKey, current + 1);
        }
    }
}
