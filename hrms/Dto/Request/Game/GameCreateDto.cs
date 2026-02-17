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
        
        public int MaxPlayer { get; set; }
        
        [Required(ErrorMessage = "Min Player can not be empty !")]
        public int MinPlayer { get; set; }
    }
}
