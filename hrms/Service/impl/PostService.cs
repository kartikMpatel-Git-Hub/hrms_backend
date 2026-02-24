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

namespace hrms.Service.impl
{
    public class PostService(
        IPostRepository _repository,
        IUserRepository _userRepository,
        IMapper _mapper,
        INotificationRepository _notificationRepository,
        IEmailService _emailService,
        ICloudinaryService _cloudinaryService
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
            return _mapper.Map<PostResponseDto>(createdPost);
        }

        public async Task<TagResponseDto> CreateTag(TagCreateDto tagCreateDto)
        {
            Tag tag = _mapper.Map<Tag>(tagCreateDto);
            Tag createdTag = await _repository.CreateTag(tag);
            return _mapper.Map<TagResponseDto>(createdTag);
        }

        public async Task DeleteComment(int postId, int commentId)
        {
            PostComment comment = await _repository.GetCommentById(commentId);
            await _repository.DeleteComment(comment);
        }

        public async Task DeletePost(int postId)
        {
            await _repository.DeletePost(postId);
        }

        public async Task DeleteTag(int tagId)
        {
            await _repository.DeleteTag(tagId);
        }

        public async Task<PagedReponseDto<CommentResponseDto>> GetCommentsForPost(int postId, int pageNumber, int pageSize)
        {
            PagedReponseOffSet<PostComment> pagedComments = await _repository.GetCommentsForPost(postId, pageNumber, pageSize);
            return _mapper.Map<PagedReponseDto<CommentResponseDto>>(pagedComments);

        }

        public async Task<PagedReponseDto<PostResponseDto>> GetFeed(int userId, int pageNumber, int pageSize)
        {
            PagedReponseOffSet<Post> pagedPosts = await _repository.GetFeed(pageNumber, pageSize);
            PagedReponseDto<PostResponseDto> response = new PagedReponseDto<PostResponseDto>
            {
                Data = pagedPosts.Data.Select(post => ConvertToDto(userId, post)).ToList(),
                TotalRecords = pagedPosts.TotalRecords,
                PageNumber = pagedPosts.PageNumber,
                PageSize = pagedPosts.PageSize,
                TotalPages = pagedPosts.TotalPages
            };
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
            Post post = await _repository.GetPostById(postId);
            return ConvertDetailToDto(userId, post);
        }

        public async Task<PagedReponseDto<PostResponseDto>> GetPosts(int page, int pageSize)
        {
            PagedReponseOffSet<Post> pagedPosts = await _repository.GetFeed(page, pageSize);
            return _mapper.Map<PagedReponseDto<PostResponseDto>>(pagedPosts);
        }

        public async Task<PagedReponseDto<TagResponseDto>> GetTags(string searchQuery, int pageNumber, int pageSize)
        {
            PagedReponseOffSet<Tag> pagedTags;
            if (searchQuery == "")
            {
                pagedTags = await _repository.GetTags(pageNumber, pageSize);
            }
            else
            {
                pagedTags = await _repository.GetTags(searchQuery, pageNumber, pageSize);
            }
            return _mapper.Map<PagedReponseDto<TagResponseDto>>(pagedTags);
        }

        public async Task<List<TagResponseDto>> GetTagsForPost(int postId)
        {
            List<Tag> postTags = await _repository.GetTagsForPost(postId);
            return _mapper.Map<List<TagResponseDto>>(postTags);
        }

        public async Task<bool> LikePost(int postId, int userId)
        {
            bool isLiked = await _repository.LikePost(postId, userId);
            return isLiked;
        }

        public async Task RemoveTagFromPost(int postId, int tagId)
        {
            await _repository.RemoveTagFromPost(postId, tagId);
        }

        public async Task<CommentResponseDto> UpdateComment(int postId, int commentId, CommentUpdateDto commentUpdateDto)
        {
            PostComment comment = await _repository.GetCommentById(commentId);
            if (commentUpdateDto.Comment != null)
                comment.Comment = commentUpdateDto.Comment;
            await _repository.UpdateComment(comment);
            return _mapper.Map<CommentResponseDto>(comment);
        }
        public async Task<PostResponseDto> UpdatePost(int postId, PostUpdateDto postUpdateDto)
        {
            Post post = await _repository.GetPostById(postId);
            Post updatedPost = UpdatePostEntity(post, postUpdateDto);
            await _repository.UpdatePost(updatedPost);
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
        }

        public async Task<PagedReponseDto<PostResponseDto>> GetInappropriatePosts(int page, int pageSize)
        {
            PagedReponseOffSet<Post> pagedPosts = await _repository.GetInappropriatePosts(page, pageSize);
            return _mapper.Map<PagedReponseDto<PostResponseDto>>(pagedPosts);
        }

        public async Task<PagedReponseDto<PostResponseDto>> GetMyPosts(int userId, int page, int pageSize)
        {
            PagedReponseOffSet<Post> pagedPosts = await _repository.GetMyPosts(userId, page, pageSize);
            PagedReponseDto<PostResponseDto> response = new PagedReponseDto<PostResponseDto>
            {
                Data = pagedPosts.Data.Select(post => ConvertToDto(userId, post)).ToList(),
                TotalRecords = pagedPosts.TotalRecords,
                PageNumber = pagedPosts.PageNumber,
                PageSize = pagedPosts.PageSize,
                TotalPages = pagedPosts.TotalPages
            };
            return response;
        }
    }
}