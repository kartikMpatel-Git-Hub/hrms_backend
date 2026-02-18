using hrms.Model;

namespace hrms.Dto.Request.BookingSlot
{
    public class BookSlotRequestDto
    {
        public List<int> Players { get; set; } = new List<int>();
    }
}
