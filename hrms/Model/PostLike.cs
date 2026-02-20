namespace hrms.Model
{
    public class PostLike
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public int LikedById { get; set; }
        public User LikedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}