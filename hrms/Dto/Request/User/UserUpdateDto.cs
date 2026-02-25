namespace hrms.Dto.Request.User
{
    public class UserUpdateRequestDto
    {
        public String? FullName { get; set; }
        public String? Email { get; set; }
        public IFormFile? Image { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfJoin { get; set; }
        public int? ReportTo { get; set; }
        public int? DepartmentId { get; set; }
        public string? Designation {  get; set; }
    }
}