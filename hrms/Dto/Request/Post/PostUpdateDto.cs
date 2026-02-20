namespace hrms.Dto.Request.Post
{
    public class PostUpdateDto
    {
        public IFormFile Post { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
    }
}