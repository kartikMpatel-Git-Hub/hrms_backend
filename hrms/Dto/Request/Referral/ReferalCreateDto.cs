namespace hrms.Dto.Request.Referral
{
    public class ReferralCreateDto
    {
        public string ReferedPersonName { get; set; }
        public string ReferedPersonEmail { get; set; }
        public IFormFile Cv { get; set; }
        public string Note { get; set; }
    }
}
