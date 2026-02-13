using hrms.Dto.Response.User;

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
        public UserResponseDto Uploader { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
