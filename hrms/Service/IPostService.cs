using hrms.Dto.Request.Post;
using hrms.Dto.Request.Post.Comment;
using hrms.Dto.Request.Post.Tag;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Post;
using hrms.Dto.Response.Post.Comment;
using hrms.Dto.Response.Post.Tag;

namespace hrms.Service
{
    public interface IPostService
    {
        Task AddTagToPost(int postId, int tagId);
        Task<CommentResponseDto> CommentOnPost(int postId, int userId, CommentCreateDto commentCreateDto);
        Task<PostResponseDto> CreatePost(int userId, PostCreateDto postCreateDto);
        Task<TagResponseDto> CreateTag(TagCreateDto tagCreateDto);
        Task DeleteComment(int postId, int commentId);
        Task DeletePost(int postId);
        Task DeleteTag(int tagId);
        Task<PagedReponseDto<CommentResponseDto>> GetCommentsForPost(int postId, int pageNumber, int pageSize);
        Task<PagedReponseDto<PostResponseDto>> GetFeed(int userId, int pageNumber, int pageSize);
        Task<PagedReponseDto<PostResponseDto>> GetInappropriatePosts(int page, int pageSize);
        Task<PagedReponseDto<PostResponseDto>> GetMyPosts(int userId, int page, int pageSize);
        Task<PostDetailResponseDto> GetPost(int userId, int postId);
        Task<PagedReponseDto<PostResponseDto>> GetPosts(int page, int pageSize);
        Task<PagedReponseDto<TagResponseDto>> GetTags(string searchQuery, int pageNumber, int pageSize);
        Task<List<TagResponseDto>> GetTagsForPost(int postId);
        Task<bool> LikePost(int postId, int userId);
        Task MarkPostInAppropriate(int postId, string? reason);
        Task RemoveTagFromPost(int postId, int tagId);
        Task<CommentResponseDto> UpdateComment(int postId, int commentId, CommentUpdateDto commentUpdateDto);
        Task<PostResponseDto> UpdatePost(int postId, PostUpdateDto postUpdateDto);
    }
}