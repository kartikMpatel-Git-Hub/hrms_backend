using System.ComponentModel.DataAnnotations;

namespace hrms.Dto.Request.Authentication
{
    public class RegisterRequestDto
    {
        [Required]
        [MaxLength(30)]
        public String full_name { get; set; }

        [Required]
        [EmailAddress]
        public String email { get; set; }

        [Required]
        [MinLength(8)]
        public String password { get; set; }
        public String image_url { get; set; }

        [Required]
        public String user_role { get; set; }
        public DateOnly date_of_birth { get; set; }
        public DateOnly date_of_join { get; set; }
        public int? manage_id { get; set; }
    }
}
