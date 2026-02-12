using hrms.Model;
using hrms.Repository;

namespace hrms.Service.impl
{
    public class JobReviewerService : IJobReviewerService
    {

        private readonly IUserRepository _userRepository;
        private readonly IJobReviewerRepository _jobReviewerRepository;

        public JobReviewerService(IUserRepository userRepository, IJobReviewerRepository jobReviewerRepository)
        {
            _userRepository = userRepository;
            _jobReviewerRepository = jobReviewerRepository;
        }

        public async Task AddReviewers(Job createdJob, List<int> reviewers)
        {
            foreach (var r in reviewers)
            {
                User rw = await _userRepository.GetByIdAsync(r);
                JobReviewer reviewer = new JobReviewer()
                {
                    JobId = createdJob.Id,
                    Job = createdJob,
                    Reviewer = rw,
                    ReviewerId = rw.Id
                };
                await _jobReviewerRepository.AddReviewer(reviewer);
            }
        }
    }
}
