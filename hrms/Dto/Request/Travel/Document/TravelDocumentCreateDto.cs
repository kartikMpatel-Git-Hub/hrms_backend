using hrms.Model;

namespace hrms.Dto.Request.Travel.Document
{
    public class TravelDocumentCreateDto
    {
        public string DocumentName { get; set; }
        public IFormFile? Document { get; set; }
    }
}
