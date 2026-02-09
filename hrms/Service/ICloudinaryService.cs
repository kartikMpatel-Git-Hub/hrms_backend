namespace hrms.Service
{
    public interface ICloudinaryService
    {
        Task<string> UploadAsync(IFormFile file);
        Task DeleteAsync(string publicId);
    }
}
