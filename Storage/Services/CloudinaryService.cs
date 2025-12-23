using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Utilities;
using Microsoft.Extensions.Logging;

namespace Storage.Services
{
    public interface ICloudinaryService
    {
        Task<string> SaveMediaFileAsync(string folderPath, FileStream stream, string fileExtension);
        Task<bool> RemoveResourceAsync(string folderPath, string resourceId);
    }

    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(ILogger<CloudinaryService> logger)
        {
            _cloudinary = new Cloudinary(new Account
            {
                Cloud = EnvUtil.GetEnv(EnvUtil.Keys.HOANGCN_CLOUDINARY_NAME),
                ApiKey = EnvUtil.GetEnv(EnvUtil.Keys.HOANGCN_CLOUDINARY_API_KEY),
                ApiSecret = EnvUtil.GetEnv(EnvUtil.Keys.HOANGCN_CLOUDINARY_API_SECRET),
            });
            _cloudinary.Api.Secure = true;
            _logger = logger;

            _logger.LogInformation("[CloudinaryService] Initialized");
        }

        public async Task<bool> RemoveResourceAsync(string folderPath, string fileName)
        {
            string publicId = folderPath + Path.GetFileName(fileName);
            var result = await _cloudinary.DeleteResourcesAsync(ResourceType.Raw, publicId);
            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<string> SaveMediaFileAsync(string folderPath, FileStream stream, string fileExtension)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var uploadParam = new RawUploadParams
            {
                Folder = folderPath,
                File = new FileDescription(Guid.NewGuid() + fileExtension, stream)
            };

            var result = await _cloudinary.UploadLargeAsync(uploadParam);
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Upload failed: " + result.Error.Message);
            }
            _logger.LogInformation("[CloudinaryService] Đã upload:" + uploadParam.File.FileName);
            
            return result.SecureUrl.ToString();
        }
    }
}
