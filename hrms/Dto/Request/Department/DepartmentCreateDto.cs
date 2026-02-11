using System.ComponentModel.DataAnnotations;

namespace hrms.Dto.Request.Department
{
    public class DepartmentCreateDto
    {
        [Required(ErrorMessage ="Department can't be empty")]
        [MinLength(2,ErrorMessage ="Department Must have Atleast 2 character")]
        public string DepartmentName { get; set; }
    }
}
