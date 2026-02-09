using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace hrms.Model
{
    public class Travel : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Desciption { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public int CreatedBy { get; set; }
        public User Creater { get; set; }
        public decimal MaxAmountLimit { get; set;}
        public List<Traveler> Travelers { get; set; } = new List<Traveler>();
    }
}
