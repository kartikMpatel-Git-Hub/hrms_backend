namespace hrms.Model
{
    public class BaseEntity
    {
        public bool is_deleted { get; set; } = false;
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}