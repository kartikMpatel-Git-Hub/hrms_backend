using hrms.Dto.Response.User;

namespace hrms.Dto.Response.Game
{
    public class GameSlotPlayerResponseDto
    {
        public int Id { get; set; }
        public int SlotId { get; set; }
        public int PlayerId { get; set; }
        public UserMinimalResponseDto Player { get; set; }
    }
}