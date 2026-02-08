namespace hrms.Model
{
    public class Travel : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Desciption { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Location { get; set; }
        public int CreatedBy { get; set; }
        public User Creater { get; set; }
        public decimal MaxAmountLimit { get; set;}

    }
}
