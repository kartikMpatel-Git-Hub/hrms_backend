using hrms.Dto.Response.User;

namespace hrms.Dto.Response.Game
{
    public class WaitlistPlayerResponseDto
    {
        public int Id { get; set; }
        public int GameSlotWaitingId { get; set; }
        public int PlayerId { get; set; }
        public UserMinimalResponseDto Player { get; set; }
    }
}