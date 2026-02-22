namespace hrms.Model
{
    public class PostComment : BaseEntity
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public int CommentById { get; set; }
        public User CommentBy { get; set; }
    }
}