using AutoMapper;
using hrms.Dto.Request.Job;
using hrms.Dto.Response.Job;
using hrms.Dto.Response.Other;
using hrms.Model;
using hrms.Repository;

namespace hrms.Service.impl
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _repository;
        private readonly ICloudinaryService _cloudinary;
        private readonly IUserRepository _userRepository;
        private readonly IJobReviewerService _jobReviewerService;
        private readonly IMapper _mappers;

        public JobService(IJobRepository repository, ICloudinaryService cloudinary
            , IJobReviewerService jobReviewerService, IMapper mapper, IUserRepository userRepository)
        {
            _repository = repository;
            _cloudinary = cloudinary;
            _mappers = mapper;
            _userRepository = userRepository;
            _jobReviewerService = jobReviewerService;
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

        public async Task<JobResponseWithReviewersDto> GetJobDetail(int jobId)
        {
            Job job = await _repository.GetJobById(jobId);
            return _mappers.Map<JobResponseWithReviewersDto>(job);  
        }

        public async Task<PagedReponseDto<JobResponseDto>> GetJobsCreatedByHr(int hrId,int pageNumber,int pageSize)
        {
            User hr = await _userRepository.GetHrById(hrId);
            PagedReponseOffSet<Job> jobs = await _repository.GetJobsCreatedByHr(hrId,pageNumber,pageSize);
            return _mappers.Map<PagedReponseDto<JobResponseDto>>(jobs);
        }
    }
}
