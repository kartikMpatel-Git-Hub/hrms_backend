using AutoMapper;
using Azure;
using hrms.Dto.Request.Post;
using hrms.Dto.Request.Post.Comment;
using hrms.Dto.Request.Post.Tag;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Post;
using hrms.Dto.Response.Post.Comment;
using hrms.Dto.Response.Post.Tag;
using hrms.Model;
using hrms.Repository;
using hrms.Utility;
using Microsoft.Extensions.Caching.Memory;

namespace hrms.Service.impl
{
    public class PostService(
        IPostRepository _repository,
        IUserRepository _userRepository,
        IMapper _mapper,
        INotificationRepository _notificationRepository,
        IEmailService _emailService,
        ICloudinaryService _cloudinaryService,
        IMemoryCache _cache,
        ILogger<PostService> _logger
        ) : IPostService
    {
        public async Task AddTagToPost(int postId, int tagId)
        {
            await _repository.GetPostById(postId);
            await _repository.GetTagById(tagId);
            PostTag postTag = new PostTag()
            {
                PostId = postId,
                TagId = tagId
            };
            await _repository.AddTagToPost(postTag);
            IncrementCacheVersion(CacheVersionKey.ForPostInfo(postId));
            _logger.LogInformation("Tag {TagId} added to post {PostId}, cache version incremented", tagId, postId);
        }

        public async Task<CommentResponseDto> CommentOnPost(int postId, int userId, CommentCreateDto commentCreateDto)
        {
            await _userRepository.GetByIdAsync(userId);
            await _repository.GetPostById(postId);
            PostComment comment = _mapper.Map<PostComment>(commentCreateDto);
            comment.PostId = postId;
            comment.CommentById = userId;
            comment.created_at = DateTime.Now;
            comment.updated_at = DateTime.Now;
            PostComment createdComment = await _repository.CreateComment(comment);
            IncrementCacheVersion(CacheVersionKey.ForPostComments(postId));
            _logger.LogInformation("Comment created on post {PostId} by user {UserId}, cache version incremented", postId, userId);
            return _mapper.Map<CommentResponseDto>(createdComment);
        }

        public async Task<PostResponseDto> CreatePost(int userId, PostCreateDto postCreateDto)
        {
            User user = await _userRepository.GetByIdAsync(userId);
            Post post = await CreatePost(postCreateDto);
            post.PostById = userId;
            post.PostUrl = await _cloudinaryService.UploadAsync(postCreateDto.Post);
            Post createdPost = await _repository.CreatePost(post);
            createdPost.PostByUser = user;
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.PostDetails));
            IncrementCacheVersion(CacheVersionKey.ForUserPosts(userId));
            _logger.LogInformation("Post created with Id {PostId}, cache versions incremented", createdPost.Id);
            return _mapper.Map<PostResponseDto>(createdPost);
        }

        public async Task<TagResponseDto> CreateTag(TagCreateDto tagCreateDto)
        {
            Tag tag = _mapper.Map<Tag>(tagCreateDto);
            Tag createdTag = await _repository.CreateTag(tag);
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.PostTags));
            _logger.LogInformation("Tag created with Id {TagId}, cache version incremented", createdTag.Id);
            return _mapper.Map<TagResponseDto>(createdTag);
        }

        public async Task DeleteComment(int postId, int commentId)
        {
            PostComment comment = await _repository.GetCommentById(commentId);
            await _repository.DeleteComment(comment);
            IncrementCacheVersion(CacheVersionKey.ForPostComments(postId));
            _logger.LogInformation("Comment {CommentId} deleted from post {PostId}, cache version incremented", commentId, postId);
        }

        public async Task DeletePost(int postId)
        {
            await _repository.DeletePost(postId);
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.PostDetails));
            IncrementCacheVersion(CacheVersionKey.ForPostInfo(postId));
            _logger.LogInformation("Post {PostId} deleted, cache versions incremented", postId);
        }

        public async Task DeleteTag(int tagId)
        {
            await _repository.DeleteTag(tagId);
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.PostTags));
            _logger.LogInformation("Tag {TagId} deleted, cache version incremented", tagId);
        }

        public async Task<PagedReponseDto<CommentResponseDto>> GetCommentsForPost(int postId, int pageNumber, int pageSize)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForPostComments(postId));
            var cacheKey = $"PostComments:PostId:{postId}:page:{pageNumber}:size:{pageSize}:version:{version}";
            if (_cache.TryGetValue(cacheKey, out PagedReponseDto<CommentResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for post {PostId} comments (version {Version})", postId, version);
                return cached;
            }
            PagedReponseOffSet<PostComment> pagedComments = await _repository.GetCommentsForPost(postId, pageNumber, pageSize);
            var result = _mapper.Map<PagedReponseDto<CommentResponseDto>>(pagedComments);
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Retrieved comments for post {PostId} and cached with version {Version}", postId, version);
            return result;
        }

        public async Task<PagedReponseDto<PostResponseDto>> GetFeed(int userId, int pageNumber, int pageSize)
        {
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.PostDetails));
            var cacheKey = $"PostFeed:UserId:{userId}:page:{pageNumber}:size:{pageSize}:version:{version}";
            if (_cache.TryGetValue(cacheKey, out PagedReponseDto<PostResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for feed userId {UserId} (version {Version})", userId, version);
                return cached;
            }
            PagedReponseOffSet<Post> pagedPosts = await _repository.GetFeed(pageNumber, pageSize);
            PagedReponseDto<PostResponseDto> response = new PagedReponseDto<PostResponseDto>
            {
                Data = pagedPosts.Data.Select(post => ConvertToDto(userId, post)).ToList(),
                TotalRecords = pagedPosts.TotalRecords,
                PageNumber = pagedPosts.PageNumber,
                PageSize = pagedPosts.PageSize,
                TotalPages = pagedPosts.TotalPages
            };
            _cache.Set(cacheKey, response, TimeSpan.FromMinutes(10));
            _logger.LogInformation("Retrieved feed for user {UserId} and cached with version {Version}", userId, version);
            return response;
        }

        private PostResponseDto ConvertToDto(int userId, Post post)
        {
            PostResponseDto dto = _mapper.Map<PostResponseDto>(post);
            dto.IsLiked = post.Likes.Any(like => like.LikedById == userId);
            return dto;
        }

        private PostDetailResponseDto ConvertDetailToDto(int userId, Post post)
        {
            PostDetailResponseDto dto = _mapper.Map<PostDetailResponseDto>(post);
            dto.IsLiked = post.Likes.Any(like => like.LikedById == userId);
            dto.Tags = _mapper.Map<List<TagResponseDto>>(post.PostTags.Select(pt => pt.Tag).ToList());
            return dto;
        }

        public async Task<PostDetailResponseDto> GetPost(int userId, int postId)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForPostInfo(postId));
            var cacheKey = $"PostDetail:PostId:{postId}:UserId:{userId}:version:{version}";
            if (_cache.TryGetValue(cacheKey, out PostDetailResponseDto cached))
            {
                _logger.LogDebug("Cache hit for post {PostId} detail (version {Version})", postId, version);
                return cached;
            }
            Post post = await _repository.GetPostById(postId);
            var result = ConvertDetailToDto(userId, post);
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Retrieved post {PostId} detail and cached with version {Version}", postId, version);
            return result;
        }

        public async Task<PagedReponseDto<PostResponseDto>> GetPosts(int page, int pageSize)
        {
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.PostDetails));
            var cacheKey = $"Posts:page:{page}:size:{pageSize}:version:{version}";
            if (_cache.TryGetValue(cacheKey, out PagedReponseDto<PostResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for posts list (version {Version})", version);
                return cached;
            }
            PagedReponseOffSet<Post> pagedPosts = await _repository.GetFeed(page, pageSize);
            var result = _mapper.Map<PagedReponseDto<PostResponseDto>>(pagedPosts);
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Retrieved posts list and cached with version {Version}", version);
            return result;
        }

        public async Task<PagedReponseDto<TagResponseDto>> GetTags(string searchQuery, int pageNumber, int pageSize)
        {
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.PostTags));
            var cacheKey = $"Tags:search:{searchQuery}:page:{pageNumber}:size:{pageSize}:version:{version}";
            if (_cache.TryGetValue(cacheKey, out PagedReponseDto<TagResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for tags (version {Version})", version);
                return cached;
            }
            PagedReponseOffSet<Tag> pagedTags;
            if (searchQuery == "")
            {
                pagedTags = await _repository.GetTags(pageNumber, pageSize);
            }
            else
            {
                pagedTags = await _repository.GetTags(searchQuery, pageNumber, pageSize);
            }
            var result = _mapper.Map<PagedReponseDto<TagResponseDto>>(pagedTags);
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(60));
            _logger.LogInformation("Retrieved tags and cached with version {Version}", version);
            return result;
        }

        public async Task<List<TagResponseDto>> GetTagsForPost(int postId)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForPostInfo(postId));
            var cacheKey = $"PostTags:PostId:{postId}:version:{version}";
            if (_cache.TryGetValue(cacheKey, out List<TagResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for post {PostId} tags (version {Version})", postId, version);
                return cached;
            }
            List<Tag> postTags = await _repository.GetTagsForPost(postId);
            var result = _mapper.Map<List<TagResponseDto>>(postTags);
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Retrieved tags for post {PostId} and cached with version {Version}", postId, version);
            return result;
        }

        public async Task<bool> LikePost(int postId, int userId)
        {
            bool isLiked = await _repository.LikePost(postId, userId);
            IncrementCacheVersion(CacheVersionKey.ForPostInfo(postId));
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.PostDetails));
            _logger.LogInformation("Post {PostId} like toggled by user {UserId}, cache versions incremented", postId, userId);
            return isLiked;
        }

        public async Task RemoveTagFromPost(int postId, int tagId)
        {
            await _repository.RemoveTagFromPost(postId, tagId);
            IncrementCacheVersion(CacheVersionKey.ForPostInfo(postId));
            _logger.LogInformation("Tag {TagId} removed from post {PostId}, cache version incremented", tagId, postId);
        }

        public async Task<CommentResponseDto> UpdateComment(int postId, int commentId, CommentUpdateDto commentUpdateDto)
        {
            PostComment comment = await _repository.GetCommentById(commentId);
            if (commentUpdateDto.Comment != null)
                comment.Comment = commentUpdateDto.Comment;
            await _repository.UpdateComment(comment);
            IncrementCacheVersion(CacheVersionKey.ForPostComments(postId));
            _logger.LogInformation("Comment {CommentId} updated on post {PostId}, cache version incremented", commentId, postId);
            return _mapper.Map<CommentResponseDto>(comment);
        }
        public async Task<PostResponseDto> UpdatePost(int postId, PostUpdateDto postUpdateDto)
        {
            Post post = await _repository.GetPostById(postId);
            Post updatedPost = UpdatePostEntity(post, postUpdateDto);
            await _repository.UpdatePost(updatedPost);
            IncrementCacheVersion(CacheVersionKey.ForPostInfo(postId));
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.PostDetails));
            _logger.LogInformation("Post {PostId} updated, cache versions incremented", postId);
            return _mapper.Map<PostResponseDto>(updatedPost);
        }
        private Post UpdatePostEntity(Post post, PostUpdateDto postUpdateDto)
        {
            if (postUpdateDto.Title != null)
                post.Title = postUpdateDto.Title;
            if (postUpdateDto.Description != null)
                post.Description = postUpdateDto.Description;
            if (postUpdateDto.IsPublic != post.IsPublic)
                post.IsPublic = postUpdateDto.IsPublic;
            return post;
        }
        private async Task<Post> CreatePost(PostCreateDto postCreateDto)
        {
            Post post = new Post()
            {
                Title = postCreateDto.Title,
                Description = postCreateDto.Description,
                IsPublic = postCreateDto.IsPublic,
                InAppropriate = false,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };
            post.PostTags = new List<PostTag>();
            if (postCreateDto.Tags != null)
            {
                foreach (string tagName in postCreateDto.Tags)
                {
                    Tag tag = await _repository.GetTagByName(tagName);
                    if (tag != null)
                    {
                        PostTag newTag = new PostTag()
                        {
                            TagId = tag.Id,
                            Tag = tag
                        };
                        post.PostTags.Add(newTag);
                    }
                }
            }
            return post;
        }

        public async Task MarkPostInAppropriate(int postId, string? reason)
        {
            Post post = await _repository.GetPostById(postId);
            if (!post.InAppropriate)
            {
                post.InAppropriateReason = reason ?? "No reason provided";
                await CreateNotificationForInAppropriate(post);
            }
            post.InAppropriate = !post.InAppropriate;
            await _repository.MarkPostInAppropriate(post);
            IncrementCacheVersion(CacheVersionKey.ForPostInfo(postId));
            IncrementCacheVersion(CacheVersionKey.For(CacheDomains.PostDetails));
            IncrementCacheVersion(CacheVersionKey.ForUserPosts(post.PostById));
            _logger.LogInformation("Post {PostId} inappropriate status toggled, cache versions incremented", postId);
        }

        private async Task CreateNotificationForInAppropriate(Post post)
        {
            await _emailService.SendEmailAsync(post.PostByUser.Email, "Inappropriate Post Reported", $"Your post titled '{post.Title}' has been reported as inappropriate.");
            Notification notification = new Notification()
            {
                NotifiedTo = post.PostById,
                Title = "Post Inappropriate",
                NotificationDate = DateTime.Now,
                Description = $"Your post titled '{post.Title}' has been reported as inappropriate. it will not longer be visible to other users until reviewed by the admin.",
            };
            await _notificationRepository.CreateNotification(notification);
            IncrementCacheVersion(CacheVersionKey.ForUserNotifications(post.PostById));
            _logger.LogInformation("Notification sent to user with Id {UserId} for inappropriate post with Id {PostId}", post.PostById, post.Id);
        }

        private void IncrementCacheVersion(string v)
        {
            var current = _cache.Get<int>(v);
            _cache.Set(v, current + 1);
        }

        public async Task<PagedReponseDto<PostResponseDto>> GetInappropriatePosts(int page, int pageSize)
        {
            var version = _cache.Get<int>(CacheVersionKey.For(CacheDomains.PostDetails));
            var cacheKey = $"InappropriatePosts:page:{page}:size:{pageSize}:version:{version}";
            if (_cache.TryGetValue(cacheKey, out PagedReponseDto<PostResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for inappropriate posts (version {Version})", version);
                return cached;
            }
            PagedReponseOffSet<Post> pagedPosts = await _repository.GetInappropriatePosts(page, pageSize);
            var result = _mapper.Map<PagedReponseDto<PostResponseDto>>(pagedPosts);
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(30));
            _logger.LogInformation("Retrieved inappropriate posts and cached with version {Version}", version);
            return result;
        }

        public async Task<PagedReponseDto<PostResponseDto>> GetMyPosts(int userId, int page, int pageSize)
        {
            var version = _cache.Get<int>(CacheVersionKey.ForUserPosts(userId));
            var cacheKey = $"MyPosts:UserId:{userId}:page:{page}:size:{pageSize}:version:{version}";
            if (_cache.TryGetValue(cacheKey, out PagedReponseDto<PostResponseDto> cached))
            {
                _logger.LogDebug("Cache hit for user {UserId} posts (version {Version})", userId, version);
                return cached;
            }
            PagedReponseOffSet<Post> pagedPosts = await _repository.GetMyPosts(userId, page, pageSize);
            PagedReponseDto<PostResponseDto> response = new PagedReponseDto<PostResponseDto>
            {
                Data = pagedPosts.Data.Select(post => ConvertToDto(userId, post)).ToList(),
                TotalRecords = pagedPosts.TotalRecords,
                PageNumber = pagedPosts.PageNumber,
                PageSize = pagedPosts.PageSize,
                TotalPages = pagedPosts.TotalPages
            };
            _cache.Set(cacheKey, response, TimeSpan.FromMinutes(15));
            _logger.LogInformation("Retrieved posts for user {UserId} and cached with version {Version}", userId, version);
            return response;
        }
    }
}