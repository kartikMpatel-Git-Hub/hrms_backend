using hrms.Model;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace hrms.Dto.Request.Travel
{
    public class TravelCreateDto
    {
        [Required(ErrorMessage ="Travel Title Required !")]
        [StringLength(50,MinimumLength = 5,ErrorMessage ="Title Must contain atlest 5 charatcer and atmost 50 character")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Travel Description Required !")]
        [StringLength(300, MinimumLength = 20, ErrorMessage = "Description Must contain atlest 20 charatcer and atmost 300 character")]
        public string Desciption { get; set; }

        [Required(ErrorMessage = "Travel Start Date Required !")]
        //[CustomValidation(typeof(TravelCreateDto), nameof(ValidateFutureDate), ErrorMessage ="Start Date Can not be In Past !")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Travel Travel End Required !")]
        //[CustomValidation(typeof(TravelCreateDto), nameof(ValidateFutureDate), ErrorMessage = "End Date Can not be In Past !")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Travel Location Required !")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Location Must contain atlest 5 charatcer and atmost 50 character")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Travel Max Amount Limit Per Day Required Required !")]
        public decimal MaxAmountLimit { get; set; }

        private object ValidateFutureDate(object value, ValidationContext context)
        {

            if (value is DateTime date)
            {
                if (date <= DateTime.Now)
                {
                    return new ValidationResult("Travel date must be in the future.");
                }
            }

            return ValidationResult.Success;

        }
    }
}
