namespace hrms.Model
{
    public enum UserRole
    {
        HR,
        MANAGER,
        EMPLOYEE,
        ADMIN
    }
    public class User : BaseEntity
    {
        public int Id { get; set; }
        public String FullName { get; set; }
        public String Email { get; set; }
        public String HashPassword { get; set; }
        public String Image { get; set; }
        public UserRole Role { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfJoin { get; set; }
        public int? ManagerId { get; set; }
        public User Manager { get; set; }
        public ICollection<User> Employees { get; set; }

    }
}
