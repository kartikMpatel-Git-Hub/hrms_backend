namespace hrms.Model
{
    public class SystemSettings
    {
        public int Id { get; set; }
        public string? BirthdayImageUrl { get; set; }
        public string? AnniversaryImageUrl { get; set; }
        public string? DefaultProfileImageUrl { get; set; }
        public int? DefaultHrId { get; set; }
        public User? DefaultHr { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }
    }
}