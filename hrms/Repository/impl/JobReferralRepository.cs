using hrms.CustomException;
using hrms.Data;
using hrms.Dto.Response.Other;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Repository.impl
{
    public class JobReferralRepository : IJobReferralRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<JobReferralRepository> _logger;

        public JobReferralRepository(ApplicationDbContext db, ILogger<JobReferralRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<JobReferral> AddReferral(JobReferral jobReferral)
        {
            var SavedEntity = await _db.Referrals.AddAsync(jobReferral);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created JobReferral with Id {Id} for JobId {JobId}", SavedEntity.Entity.Id, jobReferral.JobId);
            return SavedEntity.Entity;
        }

        public async Task<PagedReponseOffSet<JobReferral>> GetAllJobReferalByEmployeeId(int refId, int pageNumber, int pageSize)
        {
            var TotalRecords = await _db
                .Referrals
                .Where(tr => tr.ReferedBy == refId)
                .CountAsync();

            List<JobReferral> referrals = await _db.Referrals
                //.Include(jr => jr.Job)
                .Where(jr => jr.ReferedBy == refId)
                .OrderByDescending(jr => jr.ReferedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            PagedReponseOffSet<JobReferral> response = new PagedReponseOffSet<JobReferral>(referrals, pageNumber, pageSize, TotalRecords);
            _logger.LogInformation("Fetched {Count} referrals by EmployeeId {EmployeeId}, page {Page}", referrals.Count, refId, pageNumber);
            return response;
        }

        public async Task<PagedReponseOffSet<JobReferral>> GetAllJobReferalByHrId(int hrId, int pageNumber, int pageSize)
        {
            var TotalRecords = await _db
                .Referrals
                .Where(tr => tr.Job.CreatedBy == hrId)
                .CountAsync();

            List<JobReferral> referrals = await _db.Referrals
                //.Include(jr => jr.Job)
                .Where(jr => jr.Job.CreatedBy == hrId)
                .OrderByDescending(jr => jr.ReferedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            PagedReponseOffSet<JobReferral> response = new PagedReponseOffSet<JobReferral>(referrals, pageNumber, pageSize, TotalRecords);
            _logger.LogInformation("Fetched {Count} referrals by HrId {HrId}, page {Page}", referrals.Count, hrId, pageNumber);
            return response;
        }

        public async Task<PagedReponseOffSet<JobReferral>> GetAllJobReferalByJobId(int jobId, int pageNumber, int pageSize)
        {
            var TotalRecords = await _db
                .Referrals
                .Where(tr => tr.JobId == jobId)
                .CountAsync();

            List<JobReferral> referrals = await _db.Referrals
                //.Include(jr => jr.Job)
                .Where(jr => jr.JobId == jobId)
                .Include(jr => jr.Referer)
                .OrderByDescending(jr => jr.ReferedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            PagedReponseOffSet<JobReferral> response = new PagedReponseOffSet<JobReferral>(referrals, pageNumber, pageSize, TotalRecords);
            _logger.LogInformation("Fetched {Count} referrals for JobId {JobId}, page {Page}", referrals.Count, jobId, pageNumber);
            return response;
        }

        public async Task<JobReferral> GetReferralById(int referralId)
        {
            JobReferral referral = await _db.Referrals
                .Where(jr => jr.Id == referralId)
                .FirstOrDefaultAsync();
            if (referral == null) {
                throw new NotFoundCustomException($"Referral with id : {referralId} Not Found !");
            }
            _logger.LogInformation("Fetched JobReferral with Id {Id}", referralId);
            return referral;
        }

        public async Task<JobReferral> UpdateReferral(JobReferral referral)
        {
            var updatedEntity = _db.Referrals.Update(referral);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Updated JobReferral with Id {Id}", referral.Id);
            return updatedEntity.Entity;
        }
    }
}
