using hrms.CustomException;
using hrms.Data;
using hrms.Dto.Response.Other;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Repository.impl
{
    public class PostRepository(
        ApplicationDbContext _db
    ) : IPostRepository
    {
        public async Task<PostTag> AddTagToPost(PostTag postTag)
        {
            var addedPostTag = await _db.PostTags.AddAsync(postTag);
            await _db.SaveChangesAsync();
            return addedPostTag.Entity;
        }

        public async Task<PostComment> CreateComment(PostComment comment)
        {
            var addedComment = await _db.PostComments.AddAsync(comment);
            await _db.SaveChangesAsync();
            return addedComment.Entity;
        }

        public async Task<Post> CreatePost(Post post)
        {
            var addedPost = await _db.Posts.AddAsync(post);
            await _db.SaveChangesAsync();
            return addedPost.Entity;
        }

        public async Task<Tag> CreateTag(Tag tag)
        {
            var addedTag = await _db.Tags.AddAsync(tag);
            await _db.SaveChangesAsync();
            return addedTag.Entity;
        }

        public async Task DeleteComment(PostComment comment)
        {
            PostComment postComment = await GetCommentById(comment.Id);
            postComment.is_deleted = true;
            _db.PostComments.Update(postComment);
            await _db.SaveChangesAsync();
        }

        public async Task DeletePost(int postId)
        {
            Post post = await GetPostById(postId);
            post.is_deleted = true;
            _db.Posts.Update(post);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteTag(int tagId)
        {
            Tag tag = await GetTagById(tagId);
            tag.IsDeleted = true;
            _db.Tags.Update(tag);
            await _db.SaveChangesAsync();
        }

        public async Task<PostComment> GetCommentById(int commentId)
        {
            PostComment comment = await _db.PostComments
                .Where(c => c.Id == commentId && c.is_deleted == false)
                .Include(c => c.CommentBy)
                .FirstOrDefaultAsync();
            if (comment == null)
                throw new NotFoundCustomException($"Comment with Id : {commentId} not found !");
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
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(c => c.created_at)
                .ToListAsync();

            return new PagedReponseOffSet<PostComment>(comments, pageNumber, pageSize, total);
        }

        public async Task<PagedReponseOffSet<Post>> GetFeed(int pageNumber, int pageSize)
        {
            System.Console.WriteLine($"Page Number : {pageNumber} , Page Size : {pageSize}");
            int total = await _db.Posts
                .Where(p => p.is_deleted == false && p.IsPublic == true && p.InAppropriate == false)
                .CountAsync();

            var posts = await _db.Posts
                .Where(p => p.is_deleted == false && p.IsPublic == true && p.InAppropriate == false)
                .Include(p => p.PostByUser)
                .Include(p => p.Comments.Where(c => c.is_deleted == false))
                .Include(p => p.Likes.Where(l => l.IsDeleted == false))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(p => p.created_at)
                .ToListAsync();

            return new PagedReponseOffSet<Post>(posts, pageNumber, pageSize, total);
        }

        public async Task<PagedReponseOffSet<Post>> GetInappropriatePosts(int page, int pageSize)
        {
            int total = await _db.Posts
                .Where(p => p.is_deleted == false && p.InAppropriate == true)
                .CountAsync();

            var posts = await _db.Posts
                .Where(p => p.is_deleted == false && p.InAppropriate == true)
                .Skip((page - 1) * pageSize)
                .Include(p => p.PostByUser)
                .Include(p => p.Comments.Where(c => c.is_deleted == false))
                .Include(p => p.Likes.Where(l => l.IsDeleted == false))
                .Take(pageSize)
                .OrderByDescending(p => p.created_at)
                .ToListAsync();

            return new PagedReponseOffSet<Post>(posts, page, pageSize, total);
        }

        public async Task<PagedReponseOffSet<Post>> GetMyPosts(int userId, int page, int pageSize)
        {
            int total = await _db.Posts
                .Where(p => p.is_deleted == false && p.PostById == userId)
                .CountAsync();

            var posts = await _db.Posts
                .Where(p => p.is_deleted == false && p.PostById == userId)
                .Skip((page - 1) * pageSize)
                .Include(p => p.PostByUser)
                .Include(p => p.Comments.Where(c => c.is_deleted == false))
                .Include(p => p.Likes.Where(l => l.IsDeleted == false))
                .OrderByDescending(p => p.created_at)
                .Take(pageSize)
                .ToListAsync();

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
            return post;
        }

        public async Task<Tag> GetTagById(int tagId)
        {
            Tag tag = await _db.Tags
                .Where(t => t.Id == tagId && t.IsDeleted == false)
                .FirstOrDefaultAsync();
            if (tag == null)
                throw new NotFoundCustomException($"Tag with Id : {tagId} not found !");
            return tag;
        }

        public async Task<Tag> GetTagByName(string tagName)
        {
            Tag tag = await _db.Tags
                    .Where(t => t.TagName.ToLower() == tagName.Trim().ToLower() && t.IsDeleted == false)
                    .FirstOrDefaultAsync();
            if (tag == null)
                tag = await CreateTag(new Tag { TagName = tagName.Trim() });
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
                return true;
            }
        }

        public async Task MarkPostInAppropriate(Post post)
        {
            _db.Posts.Update(post);
            await _db.SaveChangesAsync();
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
        }

        public async Task<PostComment> UpdateComment(PostComment comment)
        {
            comment.updated_at = DateTime.Now;
            _db.PostComments.Update(comment);
            await _db.SaveChangesAsync();
            return await GetCommentById(comment.Id);
        }

        public async Task<Post> UpdatePost(Post updatedPost)
        {
            updatedPost.updated_at = DateTime.Now;
            _db.Posts.Update(updatedPost);
            await _db.SaveChangesAsync();
            return updatedPost;
        }
    }
}