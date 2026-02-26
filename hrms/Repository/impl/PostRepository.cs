using hrms.CustomException;
using hrms.Data;
using hrms.Dto.Response.Other;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Repository.impl
{
    public class PostRepository(
        ApplicationDbContext _db,
        ILogger<PostRepository> _logger
    ) : IPostRepository
    {
        public async Task<PostTag> AddTagToPost(PostTag postTag)
        {
            var addedPostTag = await _db.PostTags.AddAsync(postTag);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Added Tag {TagId} to Post {PostId}", postTag.TagId, postTag.PostId);
            return addedPostTag.Entity;
        }

        public async Task<PostComment> CreateComment(PostComment comment)
        {
            var addedComment = await _db.PostComments.AddAsync(comment);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created Comment with Id {Id} on Post {PostId}", addedComment.Entity.Id, comment.PostId);
            return addedComment.Entity;
        }

        public async Task<Post> CreatePost(Post post)
        {
            var addedPost = await _db.Posts.AddAsync(post);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created Post with Id {Id}", addedPost.Entity.Id);
            return addedPost.Entity;
        }

        public async Task<Tag> CreateTag(Tag tag)
        {
            var addedTag = await _db.Tags.AddAsync(tag);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created Tag with Id {Id}, Name {Name}", addedTag.Entity.Id, addedTag.Entity.TagName);
            return addedTag.Entity;
        }

        public async Task DeleteComment(PostComment comment)
        {
            PostComment postComment = await GetCommentById(comment.Id);
            postComment.is_deleted = true;
            _db.PostComments.Update(postComment);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Soft-deleted Comment with Id {Id}", comment.Id);
        }

        public async Task DeletePost(int postId)
        {
            Post post = await GetPostById(postId);
            post.is_deleted = true;
            _db.Posts.Update(post);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Soft-deleted Post with Id {Id}", postId);
        }

        public async Task DeleteTag(int tagId)
        {
            Tag tag = await GetTagById(tagId);
            tag.IsDeleted = true;
            _db.Tags.Update(tag);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Soft-deleted Tag with Id {Id}", tagId);
        }

        public async Task<PostComment> GetCommentById(int commentId)
        {
            PostComment comment = await _db.PostComments
                .Where(c => c.Id == commentId && c.is_deleted == false)
                .Include(c => c.CommentBy)
                .FirstOrDefaultAsync();
            if (comment == null)
                throw new NotFoundCustomException($"Comment with Id : {commentId} not found !");
            _logger.LogInformation("Fetched Comment with Id {Id}", commentId);
            return comment;
        }

        public async Task<PagedReponseOffSet<PostComment>> GetCommentsForPost(int postId, int pageNumber, int pageSize)
        {
            int total = await _db.PostComments
                .Where(c => c.PostId == postId && c.is_deleted == false)
                .CountAsync();

            var comments = await _db.PostComments
                .Where(c => c.PostId == postId && c.is_deleted == false)
                .Include(c => c.CommentBy)
                .OrderByDescending(c => c.created_at)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} comments for Post {PostId}, page {Page}", comments.Count, postId, pageNumber);
            return new PagedReponseOffSet<PostComment>(comments, pageNumber, pageSize, total);
        }

        public async Task<PagedReponseOffSet<Post>> GetFeed(int pageNumber, int pageSize)
        {
            int total = await _db.Posts
                .Where(p => p.is_deleted == false && p.IsPublic == true && p.InAppropriate == false)
                .CountAsync();

            var posts = await _db.Posts
                .Where(p => p.is_deleted == false && p.IsPublic == true && p.InAppropriate == false)
                .Include(p => p.PostByUser)
                .Include(p => p.Comments.Where(c => c.is_deleted == false))
                .Include(p => p.Likes.Where(l => l.IsDeleted == false))
                .OrderByDescending(p => p.created_at)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} feed posts, page {Page}", posts.Count, pageNumber);
            return new PagedReponseOffSet<Post>(posts, pageNumber, pageSize, total);
        }

        public async Task<PagedReponseOffSet<Post>> GetInappropriatePosts(int page, int pageSize)
        {
            int total = await _db.Posts
                .Where(p => p.is_deleted == false && p.InAppropriate == true)
                .CountAsync();

            var posts = await _db.Posts
                .Where(p => p.is_deleted == false && p.InAppropriate == true)
                .OrderByDescending(p => p.created_at)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.PostByUser)
                .Include(p => p.Comments.Where(c => c.is_deleted == false))
                .Include(p => p.Likes.Where(l => l.IsDeleted == false))
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} inappropriate posts, page {Page}", posts.Count, page);
            return new PagedReponseOffSet<Post>(posts, page, pageSize, total);
        }

        public async Task<PagedReponseOffSet<Post>> GetMyPosts(int userId, int page, int pageSize)
        {
            int total = await _db.Posts
                .Where(p => p.is_deleted == false && p.PostById == userId)
                .CountAsync();

            var posts = await _db.Posts
                .Where(p => p.is_deleted == false && p.PostById == userId)
                .OrderByDescending(p => p.created_at)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.PostByUser)
                .Include(p => p.Comments.Where(c => c.is_deleted == false))
                .Include(p => p.Likes.Where(l => l.IsDeleted == false))
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} posts for UserId {UserId}, page {Page}", posts.Count, userId, page);
            return new PagedReponseOffSet<Post>(posts, page, pageSize, total);
        }

        public async Task<Post> GetPostById(int postId)
        {
            Post post = await _db.Posts
                .Where(p => p.Id == postId && p.is_deleted == false)
                .Include(p => p.Comments.Where(c => c.is_deleted == false))
                .Include(p => p.PostByUser)
                .Include(p => p.Likes.Where(l => l.IsDeleted == false))
                .Include(p => p.PostTags.Where(pt => pt.IsDeleted == false))
                    .ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync();
            if (post == null)
                throw new NotFoundCustomException($"Post with Id : {postId} not found !");
            _logger.LogInformation("Fetched Post with Id {Id}", postId);
            return post;
        }

        public async Task<Tag> GetTagById(int tagId)
        {
            Tag tag = await _db.Tags
                .Where(t => t.Id == tagId && t.IsDeleted == false)
                .FirstOrDefaultAsync();
            if (tag == null)
                throw new NotFoundCustomException($"Tag with Id : {tagId} not found !");
            _logger.LogInformation("Fetched Tag with Id {Id}", tagId);
            return tag;
        }

        public async Task<Tag> GetTagByName(string tagName)
        {
            Tag tag = await _db.Tags
                    .Where(t => t.TagName.ToLower() == tagName.Trim().ToLower() && t.IsDeleted == false)
                    .FirstOrDefaultAsync();
            if (tag == null)
                tag = await CreateTag(new Tag { TagName = tagName.Trim() });
            _logger.LogInformation("Resolved Tag by Name {Name}, Id {Id}", tagName, tag.Id);
            return tag;
        }

        public async Task<PagedReponseOffSet<Tag>> GetTags(int pageNumber, int pageSize)
        {
            int total = await _db.Tags
                .Where(t => t.IsDeleted == false)
                .CountAsync();

            var tags = await _db.Tags
                .Where(t => t.IsDeleted == false)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} tags, page {Page}", tags.Count, pageNumber);
            return new PagedReponseOffSet<Tag>(tags, pageNumber, pageSize, total);
        }

        public async Task<PagedReponseOffSet<Tag>> GetTags(string searchQuery, int pageNumber, int pageSize)
        {
            int total = await _db.Tags
                .Where(t => t.IsDeleted == false && t.TagName.Contains(searchQuery))
                .CountAsync();

            var tags = await _db.Tags
                .Where(t => t.IsDeleted == false && t.TagName.Contains(searchQuery))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} tags matching '{Query}', page {Page}", tags.Count, searchQuery, pageNumber);
            return new PagedReponseOffSet<Tag>(tags, pageNumber, pageSize, total);
        }

        public async Task<List<Tag>> GetTagsForPost(int postId)
        {
            return await _db.PostTags
                .Where(pt => pt.PostId == postId && pt.IsDeleted == false)
                .Include(pt => pt.Tag)
                .Select(p => p.Tag)
                .ToListAsync();
        }

        public async Task<bool> LikePost(int postId, int userId)
        {
            PostLike existingLike = await _db.PostLikes
                .Where(pl => pl.PostId == postId && pl.LikedById == userId)
                .FirstOrDefaultAsync();
            if (existingLike != null)
            {
                existingLike.IsDeleted = !existingLike.IsDeleted;
                _db.PostLikes.Update(existingLike);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Toggled like on Post {PostId} by User {UserId}, IsLiked: {IsLiked}", postId, userId, !existingLike.IsDeleted);
                return !existingLike.IsDeleted;
            }
            else
            {
                PostLike newLike = new PostLike
                {
                    PostId = postId,
                    LikedById = userId,
                    IsDeleted = false
                };
                await _db.PostLikes.AddAsync(newLike);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Created new like on Post {PostId} by User {UserId}", postId, userId);
                return true;
            }
        }

        public async Task MarkPostInAppropriate(Post post)
        {
            _db.Posts.Update(post);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Marked Post {Id} as InAppropriate: {InAppropriate}", post.Id, post.InAppropriate);
        }

        public async Task RemoveTagFromPost(int postId, int tagId) // check once
        {
            List<PostTag> postTag = await _db.PostTags.Where(pt => pt.PostId == postId && pt.TagId == tagId && pt.IsDeleted == false).ToListAsync();
            if (postTag == null || postTag.Count == 0)
                throw new NotFoundCustomException($"Tag with Id : {tagId} is not associated with Post with Id : {postId} !");
            foreach (var pt in postTag)
            {
                pt.IsDeleted = true;
                _db.PostTags.Update(pt);
            }
            await _db.SaveChangesAsync();
            _logger.LogInformation("Removed Tag {TagId} from Post {PostId}", tagId, postId);
        }

        public async Task<PostComment> UpdateComment(PostComment comment)
        {
            comment.updated_at = DateTime.Now;
            _db.PostComments.Update(comment);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Updated Comment with Id {Id}", comment.Id);
            return await GetCommentById(comment.Id);
        }

        public async Task<Post> UpdatePost(Post updatedPost)
        {
            updatedPost.updated_at = DateTime.Now;
            _db.Posts.Update(updatedPost);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Updated Post with Id {Id}", updatedPost.Id);
            return updatedPost;
        }
    }
}