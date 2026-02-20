using hrms.Dto.Response.User;

namespace hrms.Dto.Response.Post.Comment
{
    public class CommentResponseDto
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public UserMinimalResponseDto CommentBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}