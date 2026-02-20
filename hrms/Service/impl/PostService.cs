using AutoMapper;
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
        ICloudinaryService _cloudinaryService
        ) : IPostService
    {
        public Task AddTagToPost(int postId, int tagId)
        {
            throw new NotImplementedException();
        }

        public Task<CommentResponseDto> CommentOnPost(int postId, int userId, CommentCreateDto commentCreateDto)
        {
            throw new NotImplementedException();
        }

        public async Task<PostResponseDto> CreatePost(int userId, PostCreateDto postCreateDto)
        {
            User user = await _userRepository.GetByIdAsync(userId);
            Post post = CreatePost(postCreateDto);
            post.PostById = userId;
            post.PostUrl = await _cloudinaryService.UploadAsync(postCreateDto.Post);
            Post createdPost = await _repository.CreatePost(post);
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

        public async Task<PagedReponseDto<PostResponseDto>> GetFeed(int pageNumber, int pageSize)
        {
            PagedReponseOffSet<Post> pagedPosts = await _repository.GetFeed(pageNumber, pageSize);
            return _mapper.Map<PagedReponseDto<PostResponseDto>>(pagedPosts);
        }

        public async Task<PostDetailResponseDto> GetPost(int postId)
        {
            Post post = await _repository.GetPostById(postId);
            return _mapper.Map<PostDetailResponseDto>(post);
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
            List<PostTag> postTags = await _repository.GetTagsForPost(postId);
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
            if (comment != null)
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
        private Post CreatePost(PostCreateDto postCreateDto)
        {
            Post post = new Post()
            {
                Title = postCreateDto.Title,
                Description = postCreateDto.Description,
                IsPublic = postCreateDto.IsPublic,
                InAppropriate = false
            };
            return post;
        }

    }
}