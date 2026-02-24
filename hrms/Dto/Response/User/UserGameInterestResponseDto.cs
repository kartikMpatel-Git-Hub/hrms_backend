using hrms.Dto.Response.Game;
using hrms.Model;

namespace hrms.dto.response.user
{
    public class UserGameInterestResponseDto
    {
        public int Id { get; set; }
        public GameResponseDto Game { get; set; }
    }
}