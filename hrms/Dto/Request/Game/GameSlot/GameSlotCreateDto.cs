using System.ComponentModel.DataAnnotations;

namespace hrms.Dto.Request.Game.GameSlot
{
    public class GameSlotCreateDto
    {
        [Required(ErrorMessage = "Game slot start time can not be empty !")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "Game slot end time can not be empty !")]
        public TimeOnly EndTime { get; set; }
    }
}
