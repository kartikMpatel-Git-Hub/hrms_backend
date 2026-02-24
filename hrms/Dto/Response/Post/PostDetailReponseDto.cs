using hrms.Dto.Response.Post.Comment;
using hrms.Dto.Response.Post.Tag;
using hrms.Dto.Response.User;

namespace hrms.Dto.Response.Post
{
    public class PostDetailResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public string PostUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsLiked { get; set; }
        public UserMinimalResponseDto PostByUser { get; set; }
        public bool InAppropriate { get; set; }
        public string? InAppropriateReason { get; set; }
        public List<TagResponseDto> Tags { get; set; } = new List<TagResponseDto>();
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
}