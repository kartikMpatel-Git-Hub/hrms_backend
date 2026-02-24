namespace hrms.Model
{
    public class Post : BaseEntity
    {
        public int Id { get; set; }
        public string PostUrl { get; set; }
        public int PostById { get; set; }
        public User PostByUser { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; } = true;
        public bool InAppropriate { get; set; }
        public string? InAppropriateReason { get; set; }
        public List<PostComment> Comments { get; set; }
        public List<PostLike> Likes { get; set; }
        public List<PostTag> PostTags { get; set; }
    }
}