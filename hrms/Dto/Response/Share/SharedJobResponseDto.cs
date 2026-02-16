namespace hrms.Dto.Response.Share
{
    public class SharedJobResponseDto
    {
        public int Id { get; set; }
        public string SharedTo { get; set; }
        public int SharedBy { get; set; }
        public int JobId { get; set; }
        public DateTime SharedAt { get; set; }
    }
}
