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

        public JobReferralRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<JobReferral> AddReferral(JobReferral jobReferral)
        {
            var SavedEntity = await _db.Referrals.AddAsync(jobReferral);
            await _db.SaveChangesAsync();
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
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(jr => jr.ReferedAt)
                .ToListAsync();
            PagedReponseOffSet<JobReferral> response = new PagedReponseOffSet<JobReferral>(referrals, pageNumber, pageSize, TotalRecords);
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
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(jr => jr.ReferedAt)
                .ToListAsync();
            PagedReponseOffSet<JobReferral> response = new PagedReponseOffSet<JobReferral>(referrals, pageNumber, pageSize, TotalRecords);
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
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(jr => jr.ReferedAt)
                .ToListAsync();
            PagedReponseOffSet<JobReferral> response = new PagedReponseOffSet<JobReferral>(referrals, pageNumber, pageSize, TotalRecords);
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
            return referral;
        }
    }
}
