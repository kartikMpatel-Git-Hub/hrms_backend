using hrms.Model;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace hrms.Dto.Request.Travel
{
    public class TravelUpdateDto
    {
        [StringLength(50,MinimumLength = 5,ErrorMessage ="Title Must contain atlest 5 charatcer and atmost 50 character")]
        public string? Title { get; set; }

        [StringLength(300, MinimumLength = 20, ErrorMessage = "Description Must contain atlest 20 charatcer and atmost 300 character")]
        public string? Desciption { get; set; }

        [CustomValidation(typeof(TravelCreateDto), "ValidateFutureDate",ErrorMessage ="Start Date Can not be In Past !")]
        public DateTime? StartDate { get; set; }

        [CustomValidation(typeof(TravelCreateDto), "ValidateFutureDate", ErrorMessage = "End Date Can not be In Past !")]
        public DateTime? EndDate { get; set; }

        [StringLength(50, MinimumLength = 5, ErrorMessage = "Location Must contain atlest 5 charatcer and atmost 50 character")]
        public string? Location { get; set; }

        public decimal? MaxAmountLimit { get; set; }
    }
}
