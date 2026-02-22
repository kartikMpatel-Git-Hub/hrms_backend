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
    public class PostController(IPostService _service) : Controller
    {

        // CRUD related posts

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePost(PostCreateDto? postCreateDto)
        {
            if(postCreateDto == null)
                return BadRequest("Post Body Not Found !");
            var user = User;
            if (user == null)
                return Unauthorized("Unauthorized Access !");
            var userId = Int32.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid)?.Value);
            PostResponseDto response = await _service.CreatePost(userId,(PostCreateDto)postCreateDto);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts(int page = 1, int pageSize = 10)
        {
            if(page <= 0 || pageSize <= 0)
                return BadRequest("Page Number and Page Size must be greater than zero !");
            var user = User;
            if (user == null)
                return Unauthorized("Unauthorized Access !");
            var userId = Int32.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid)?.Value);  
            PagedReponseDto<PostResponseDto> response = await _service.GetMyPosts(userId, page, pageSize);
            return Ok(response);
        }

        [HttpGet("inappropriate")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> GetInappropriatePosts(int page = 1, int pageSize = 10)
        {
            if(page <= 0 || pageSize <= 0)
                return BadRequest("Page Number and Page Size must be greater than zero !");
            PagedReponseDto<PostResponseDto> response = await _service.GetInappropriatePosts(page, pageSize);
            return Ok(response);
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPost(int? postId)
        {
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            if(postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            var user = User;
            if (user == null)
                return Unauthorized("Unauthorized Access !");
            var userId = Int32.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid)?.Value);  
            PostDetailResponseDto response = await _service.GetPost(userId, (int)postId);
            return Ok(response);
        }

        [HttpPatch("{postId}/inappropriate")]
        public async Task<IActionResult> MarkPostInAppropriate(int? postId)
        {
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            await _service.MarkPostInAppropriate((int)postId);
            return Ok(new { message = $"Post with ID {postId} marked as inappropriate successfully." });
        }
        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(int? postId, PostUpdateDto? postUpdateDto)
        {
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            if(postUpdateDto == null)
                return BadRequest("Post Update Body Not Found !");
            PostResponseDto response = await _service.UpdatePost((int)postId,(PostUpdateDto) postUpdateDto);
            return Ok(response);
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(int? postId)
        {
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            await _service.DeletePost((int)postId);
            return Ok(new { message = "Post deleted successfully." });
        }

        // like related posts

        [HttpPatch("{postId}/like")]
        public async Task<IActionResult> LikePost(int? postId)
        {
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            var user = User;
            if (user == null)                
                return Unauthorized("Unauthorized Access !");
            var userId = Int32.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid)?.Value);
            bool isLiked = await _service.LikePost((int)postId, userId);
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
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            if(commentCreateDto == null)
                return BadRequest("Comment Body Not Found !");
            var user = User;
            if (user == null)
                return Unauthorized("Unauthorized Access !");
            var userId = Int32.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid)?.Value);
            CommentResponseDto response = await _service.CommentOnPost((int)postId, userId, (CommentCreateDto)commentCreateDto);
            return Ok(response);
        }

        [HttpGet("{postId}/comments")]
        public async Task<IActionResult> GetCommentsForPost(int? postId,int pageNumber = 1, int pageSize = 10)
        {
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            PagedReponseDto<CommentResponseDto> response = await _service.GetCommentsForPost((int)postId,pageNumber,pageSize);
            return Ok(response);
        }

        [HttpDelete("{postId}/comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(int? postId, int? commentId)
        {
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            if(commentId == null || commentId <= 0)
                return BadRequest("Comment Id must be greater than zero !");
            await _service.DeleteComment((int)postId, (int)commentId);
            return Ok(new { message = $"Comment with ID {commentId} deleted from post with ID {postId} successfully." });
        }

        [HttpPut("{postId}/comments/{commentId}")]
        public async Task<IActionResult> UpdateComment(int? postId, int? commentId, CommentUpdateDto? commentUpdateDto)
        {
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            if(commentId == null || commentId <= 0)
                return BadRequest("Comment Id must be greater than zero !");
            if(commentUpdateDto == null)
                return BadRequest("Comment Update Body Not Found !");
            CommentResponseDto response = await _service.UpdateComment((int)postId, (int)commentId, (CommentUpdateDto)commentUpdateDto);    
            return Ok(response);
        }

        // feed related posts

        [HttpGet("feed")]
        public async Task<IActionResult> GetFeed(int pageNumber = 1, int pageSize = 10)
        {
            if(pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page Number and Page Size must be greater than zero !");
            var user = User;
            if (user == null)
                return Unauthorized("Unauthorized Access !");
            var userId = Int32.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.PrimarySid)?.Value);
            PagedReponseDto<PostResponseDto> response = await _service.GetFeed(userId,pageNumber, pageSize);
            return Ok(response);
        }

        // tags related apis

        [HttpPost("tags")]
        public async Task<IActionResult> CreateTag(TagCreateDto? tagCreateDto)
        {
            if(tagCreateDto == null)
                return BadRequest("Tag Body Not Found !");
            TagResponseDto response = await _service.CreateTag(tagCreateDto);
            return Ok(response);
        }


        [HttpGet("tags")]
        public async Task<IActionResult> GetTags(string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if(pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page Number and Page Size must be greater than zero !");
            if(string.IsNullOrEmpty(searchQuery))
                searchQuery = "";
            PagedReponseDto<TagResponseDto> response = await _service.GetTags((string)searchQuery, pageNumber, pageSize);
            return Ok(response);
        }

        [HttpDelete("tags/{tagId}")]
        public async Task<IActionResult> DeleteTag(int? tagId)
        {
            if(tagId == null || tagId <= 0)
                return BadRequest("Tag Id must be greater than zero !");

            await _service.DeleteTag((int)tagId);
            return Ok(new { message = $"Tag with ID {tagId} deleted successfully." });    
        }

        // tags related to post

        [HttpGet("{postId}/tags")]
        public async Task<IActionResult> GetTagsForPost(int? postId)
        {
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            List<TagResponseDto> response = await _service.GetTagsForPost((int)postId);
            return Ok(response);            
        }

        [HttpPost("{postId}/tags/{tagId}")]
        public async Task<IActionResult> AddTagToPost(int? postId, int? tagId)
        {
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            if(tagId == null || tagId <= 0)
                return BadRequest("Tag Id must be greater than zero !");
            await _service.AddTagToPost((int)postId, (int)tagId);
            return Ok(new { message = $"Tag with ID {tagId} added to post with ID {postId} successfully." });
        }

        [HttpDelete("{postId}/tags/{tagId}")]
        public async Task<IActionResult> RemoveTagFromPost(int? postId, int? tagId)
        {
            if(postId == null || postId <= 0)
                return BadRequest("Post Id must be greater than zero !");
            if(tagId == null || tagId <= 0)
                return BadRequest("Tag Id must be greater than zero !");
            await _service.RemoveTagFromPost((int)postId, (int)tagId);
            return Ok(new { message = $"Tag with ID {tagId} removed from post with ID {postId} successfully." });
        }
    }
}