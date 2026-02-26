using hrms.Dto.Request.Post;
using hrms.Dto.Request.Post.Comment;
using hrms.Dto.Request.Post.Tag;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Post;
using hrms.Dto.Response.Post.Comment;
using hrms.Dto.Response.Post.Tag;
using hrms.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace hrms.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    [EnableCors("MyAllowSpecificOrigins")]
    public class PostController(IPostService _service, ILogger<PostController> _logger) : Controller
    {

        // CRUD related posts

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePost(PostCreateDto? postCreateDto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(postCreateDto == null)
                return BadRequest("Post Body Not Found !");
            var user = User;
            if (user == null)
                return Unauthorized("Unauthorized Access !");
            var userId = Int32.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid)?.Value);
            PostResponseDto response = await _service.CreatePost(userId,(PostCreateDto)postCreateDto);
            _logger.LogInformation("[{Method}] {Url} - Post created by user {UserId} successfully", Request.Method, Request.Path, userId);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts(int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(page <= 0 || pageSize <= 0)
                return BadRequest("Page Number and Page Size must be greater than zero !");
            var user = User;
            if (user == null)
                return Unauthorized("Unauthorized Access !");
            var userId = Int32.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid)?.Value);  
            PagedReponseDto<PostResponseDto> response = await _service.GetMyPosts(userId, page, pageSize);
            _logger.LogInformation("[{Method}] {Url} - Fetched posts for user {UserId} successfully", Request.Method, Request.Path, userId);
            return Ok(response);
        }

        [HttpGet("inappropriate")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> GetInappropriatePosts(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page Number and Page Size must be greater than zero !");
            PagedReponseDto<PostResponseDto> response = await _service.GetInappropriatePosts(pageNumber, pageSize);
            _logger.LogInformation("[{Method}] {Url} - Fetched inappropriate posts successfully", Request.Method, Request.Path);
            return Ok(response);
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPost(int? postId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            if(postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            var user = User;
            if (user == null)
                return Unauthorized("Unauthorized Access !");
            var userId = Int32.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid)?.Value);  
            PostDetailResponseDto response = await _service.GetPost(userId, (int)postId);
            _logger.LogInformation("[{Method}] {Url} - Fetched post {PostId} successfully", Request.Method, Request.Path, postId);
            return Ok(response);
        }

        [HttpPatch("{postId}/inappropriate")]
        public async Task<IActionResult> MarkPostInAppropriate(int? postId,PostInAppriproateMarkDto? markDto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            await _service.MarkPostInAppropriate((int)postId, markDto?.Reason);
            _logger.LogInformation("[{Method}] {Url} - Post {PostId} marked as inappropriate successfully", Request.Method, Request.Path, postId);
            return Ok(new { message = $"Post with ID {postId} marked as inappropriate successfully." });
        }
        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(int? postId, PostUpdateDto? postUpdateDto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            if(postUpdateDto == null)
                return BadRequest("Post Update Body Not Found !");
            PostResponseDto response = await _service.UpdatePost((int)postId,(PostUpdateDto) postUpdateDto);
            _logger.LogInformation("[{Method}] {Url} - Post {PostId} updated successfully", Request.Method, Request.Path, postId);
            return Ok(response);
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(int? postId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            await _service.DeletePost((int)postId);
            _logger.LogInformation("[{Method}] {Url} - Post {PostId} deleted successfully", Request.Method, Request.Path, postId);
            return Ok(new { message = "Post deleted successfully." });
        }

        // like related posts

        [HttpPatch("{postId}/like")]
        public async Task<IActionResult> LikePost(int? postId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            var user = User;
            if (user == null)                
                return Unauthorized("Unauthorized Access !");
            var userId = Int32.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid)?.Value);
            bool isLiked = await _service.LikePost((int)postId, userId);
            _logger.LogInformation("[{Method}] {Url} - Post {PostId} {Action} by user {UserId} successfully", Request.Method, Request.Path, postId, isLiked ? "liked" : "unliked", userId);
            return Ok(new { message = isLiked ? "Post liked successfully." : "Post Like removed successfully." });
        }

        // [HttpGet("{postId}/like")]
        // public IActionResult GetLikeCount(int postId)
        // {
            
        // }

        // comment related posts

        [HttpPost("{postId}/comments")]
        public async Task<IActionResult> CommentOnPost(int? postId, CommentCreateDto? commentCreateDto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            if(commentCreateDto == null)
                return BadRequest("Comment Body Not Found !");
            var user = User;
            if (user == null)
                return Unauthorized("Unauthorized Access !");
            var userId = Int32.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid)?.Value);
            CommentResponseDto response = await _service.CommentOnPost((int)postId, userId, (CommentCreateDto)commentCreateDto);
            _logger.LogInformation("[{Method}] {Url} - Comment added to post {PostId} by user {UserId} successfully", Request.Method, Request.Path, postId, userId);
            return Ok(response);
        }

        [HttpGet("{postId}/comments")]
        public async Task<IActionResult> GetCommentsForPost(int? postId,int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            PagedReponseDto<CommentResponseDto> response = await _service.GetCommentsForPost((int)postId,pageNumber,pageSize);
            _logger.LogInformation("[{Method}] {Url} - Fetched comments for post {PostId} successfully", Request.Method, Request.Path, postId);
            return Ok(response);
        }

        [HttpDelete("{postId}/comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(int? postId, int? commentId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            if(commentId == null || commentId <= 0)
                return BadRequest("Comment Id must be greater than zero !");
            await _service.DeleteComment((int)postId, (int)commentId);
            _logger.LogInformation("[{Method}] {Url} - Comment {CommentId} deleted from post {PostId} successfully", Request.Method, Request.Path, commentId, postId);
            return Ok(new { message = $"Comment with ID {commentId} deleted from post with ID {postId} successfully." });
        }

        [HttpPut("{postId}/comments/{commentId}")]
        public async Task<IActionResult> UpdateComment(int? postId, int? commentId, CommentUpdateDto? commentUpdateDto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            if(commentId == null || commentId <= 0)
                return BadRequest("Comment Id must be greater than zero !");
            if(commentUpdateDto == null)
                return BadRequest("Comment Update Body Not Found !");
            CommentResponseDto response = await _service.UpdateComment((int)postId, (int)commentId, (CommentUpdateDto)commentUpdateDto);
            _logger.LogInformation("[{Method}] {Url} - Comment {CommentId} on post {PostId} updated successfully", Request.Method, Request.Path, commentId, postId);
            return Ok(response);
        }

        // feed related posts

        [HttpGet("feed")]
        public async Task<IActionResult> GetFeed(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page Number and Page Size must be greater than zero !");
            var user = User;
            if (user == null)
                return Unauthorized("Unauthorized Access !");
            var userId = Int32.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<PostResponseDto> response = await _service.GetFeed(userId,pageNumber, pageSize);
            _logger.LogInformation("[{Method}] {Url} - Fetched feed for user {UserId} successfully", Request.Method, Request.Path, userId);
            return Ok(response);
        }

        // tags related apis

        [HttpPost("tags")]
        public async Task<IActionResult> CreateTag(TagCreateDto? tagCreateDto)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(tagCreateDto == null)
                return BadRequest("Tag Body Not Found !");
            TagResponseDto response = await _service.CreateTag(tagCreateDto);
            _logger.LogInformation("[{Method}] {Url} - Tag created successfully", Request.Method, Request.Path);
            return Ok(response);
        }


        [HttpGet("tags")]
        public async Task<IActionResult> GetTags(string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page Number and Page Size must be greater than zero !");
            if(string.IsNullOrEmpty(searchQuery))
                searchQuery = "";
            PagedReponseDto<TagResponseDto> response = await _service.GetTags((string)searchQuery, pageNumber, pageSize);
            _logger.LogInformation("[{Method}] {Url} - Fetched tags successfully", Request.Method, Request.Path);
            return Ok(response);
        }

        [HttpDelete("tags/{tagId}")]
        public async Task<IActionResult> DeleteTag(int? tagId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(tagId == null || tagId <= 0)
                return BadRequest("Tag Id must be greater than zero !");

            await _service.DeleteTag((int)tagId);
            _logger.LogInformation("[{Method}] {Url} - Tag {TagId} deleted successfully", Request.Method, Request.Path, tagId);
            return Ok(new { message = $"Tag with ID {tagId} deleted successfully." });    
        }

        // tags related to post

        [HttpGet("{postId}/tags")]
        public async Task<IActionResult> GetTagsForPost(int? postId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            List<TagResponseDto> response = await _service.GetTagsForPost((int)postId);
            _logger.LogInformation("[{Method}] {Url} - Fetched {Count} tags for post {PostId} successfully", Request.Method, Request.Path, response.Count, postId);
            return Ok(response);            
        }

        [HttpPost("{postId}/tags/{tagId}")]
        public async Task<IActionResult> AddTagToPost(int? postId, int? tagId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            if(tagId == null || tagId <= 0)
                return BadRequest("Tag Id must be greater than zero !");
            await _service.AddTagToPost((int)postId, (int)tagId);
            _logger.LogInformation("[{Method}] {Url} - Tag {TagId} added to post {PostId} successfully", Request.Method, Request.Path, tagId, postId);
            return Ok(new { message = $"Tag with ID {tagId} added to post with ID {postId} successfully." });
        }

        [HttpDelete("{postId}/tags/{tagId}")]
        public async Task<IActionResult> RemoveTagFromPost(int? postId, int? tagId)
        {
            _logger.LogInformation("[{Method}] {Url} - Request received", Request.Method, Request.Path);
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            if(tagId == null || tagId <= 0)
                return BadRequest("Tag Id must be greater than zero !");
            await _service.RemoveTagFromPost((int)postId, (int)tagId);
            _logger.LogInformation("[{Method}] {Url} - Tag {TagId} removed from post {PostId} successfully", Request.Method, Request.Path, tagId, postId);
            return Ok(new { message = $"Tag with ID {tagId} removed from post with ID {postId} successfully." });
        }
    }
}