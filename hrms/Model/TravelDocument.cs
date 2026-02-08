namespace hrms.Model
{
    public class TravelDocument : BaseEntity
    {
        public int Id { get; set; }
        public int TravelId { get; set; }
        public Travel Travell { get; set; }
        public int TravelerId { get; set; }
        public User Traveler { get; set; }
        public string DocumentUrl { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public int UploadedBy { get; set; }
        public User Uploader { get; set; }

    }
}
