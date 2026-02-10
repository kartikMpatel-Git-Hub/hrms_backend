namespace hrms.Dto.Response.Travel.Document
{
    public class TravelDocumentResponseDto
    {
        public int Id { get; set; }
        public int TravelId { get; set; }
        public int TravelerId { get; set; }
        public string DocumentUrl { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public int UploadedBy { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool is_deleted { get; set; }
    }
}
