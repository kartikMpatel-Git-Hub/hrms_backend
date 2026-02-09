
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace hrms.Service.impl
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudiary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudiary = cloudinary;
        }
        public async Task DeleteAsync(string publicId)
        {
            var deleteParam = new DeletionParams(publicId);
            await _cloudiary.DestroyAsync(deleteParam);
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var uploadParam = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "uploads"
            };
            var result = await _cloudiary.UploadAsync(uploadParam);

            return result.SecureUri.ToString();
        }
    }
}
