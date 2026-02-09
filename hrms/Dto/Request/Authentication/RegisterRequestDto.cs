using System.ComponentModel.DataAnnotations;

namespace hrms.Dto.Request.Authentication
{
    public class RegisterRequestDto
    {
        [Required]
        [MaxLength(30)]
        public String FullName { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Enter Valid Email")]
        public String Email { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [MinLength(8,ErrorMessage ="Password Must Contain Atlest 8 Character")]
        public String Password { get; set; }
        public String Image { get; set; }

        [Required(ErrorMessage = "Role Is Required")]
        public String Role { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfJoin { get; set; }
        public int? ManagerId { get; set; }
    }
}
