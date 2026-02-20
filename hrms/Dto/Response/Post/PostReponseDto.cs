using hrms.Dto.Response.Post.Tag;

namespace hrms.Dto.Response.Post
{
    public class PostResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public string PostUrl { get; set; }
        public List<TagResponseDto>? Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsLiked { get; set; } = false;
    }
}