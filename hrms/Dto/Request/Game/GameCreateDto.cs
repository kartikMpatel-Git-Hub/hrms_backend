using System.ComponentModel.DataAnnotations;

namespace hrms.Dto.Request.Game
{
    public class GameCreateDto
    {
        [Required(ErrorMessage = "Name can not be empty !")]
        [MinLength(3,ErrorMessage = "Game Name atlest Contain 3 Character")]
        [MaxLength(30,ErrorMessage = "Game Name not more than 30 Character")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Max Player can not be empty !")]
        public int MaxPlayer { get; set; } = 1;
        
        [Required(ErrorMessage = "Min Player can not be empty !")]
        public int MinPlayer { get; set; } = 1;

        [Required(ErrorMessage = "Duration can not be empty !")]
        public int Duration { get; set; } = 30;
        [Required(ErrorMessage = "Slot Assigned Before X Minutes can not be empty !")]
        public int SlotAssignedBeforeMinutes { get; set; } = 30;
        [Required(ErrorMessage = "Slot Create For Next X Days can not be empty !")]
        public int SlotCreateForNextXDays { get; set; } = 7;
    }
}
