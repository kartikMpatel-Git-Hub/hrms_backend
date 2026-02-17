
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using hrms.CustomException;

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
            var extention = Path.GetExtension(file.FileName).ToLower();
            UploadResult result;
            if(extention == ".pdf" || extention == ".doc" || extention == ".docx")
            {
                var uploadParam = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "uploads"
                };
                result = await _cloudiary.UploadAsync(uploadParam);
            }
            else
            {
                var uploadParam = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "uploads"
                };
                result = await _cloudiary.UploadAsync(uploadParam);
            }

            return result.SecureUri?.ToString()
                    ?? result.Uri?.ToString()
                    ?? throw new InvalidOperationCustomException("something went wrong while uploading media !");
        }
    }
}
