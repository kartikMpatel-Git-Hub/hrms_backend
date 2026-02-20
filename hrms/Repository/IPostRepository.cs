using hrms.Dto.Response.Other;
using hrms.Model;

namespace hrms.Repository
{
    public interface IPostRepository
    {
        Task<Post> CreatePost(Post post);
        Task<Tag> CreateTag(Tag tag);
        Task DeleteComment(PostComment comment);
        Task DeletePost(int postId);
        Task<Tag> GetTagById(int tagId);
        Task DeleteTag(int tagId);
        Task<PostComment> GetCommentById(int commentId);
        Task<PagedReponseOffSet<PostComment>> GetCommentsForPost(int postId, int pageNumber, int pageSize);
        Task<PagedReponseOffSet<Post>> GetFeed(int pageNumber, int pageSize);
        Task<Post> GetPostById(int postId);
        Task<PagedReponseOffSet<Tag>> GetTags(int pageNumber, int pageSize);
        Task<PagedReponseOffSet<Tag>> GetTags(string searchQuery, int pageNumber, int pageSize);
        Task<List<PostTag>> GetTagsForPost(int postId);
        Task<bool> LikePost(int postId, int userId);
        Task RemoveTagFromPost(int postId, int tagId);
        Task<PostComment> UpdateComment(PostComment comment);
        Task<Post> UpdatePost(Post updatedPost);
    }
}