namespace hrms.Model
{
    public class Notification
    {
        public int Id { get; set; }
        public int NotifiedTo { get; set; }
        public User Notified { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsViewed { get; set; }
        public DateTime NotificationDate { get; set; }
    }
}
