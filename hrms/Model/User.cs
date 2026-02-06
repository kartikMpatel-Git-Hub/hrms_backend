namespace hrms.Model
{
    public enum UserRole
    {
        HR,
        MANAGER,
        EMPLOYEE,
        ADMIN
    }
    public class User
    {
        public int Id { get; set; }
        public String full_name { get; set; }
        public String email { get; set; }
        public String hash_password { get; set; }
        public String image_url { get; set; }
        public UserRole user_role { get; set; }
        public DateOnly date_of_birth { get; set; }
        public DateOnly date_of_join { get; set; }
        public Boolean is_deleted { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int? manager_id { get; set; }
        public User manager { get; set; }
        public ICollection<User> Employees { get; set; }

    }
}
