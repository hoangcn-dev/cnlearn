using HoangCN.Core.BL.Base;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.MainSystem.Entities;
using HoangCN.Core.DL.Interfaces;
using HoangCN.MainSystem.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoangCN.MainSystem.Services
{
    /// <summary>
    /// Dịch vụ quản lý file thực tế, kế thừa BaseBL để tái sử dụng tối đa cơ chế truy vấn CSDL
    /// </summary>
    public class FileService : BaseBL<ResourceFile>, IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileService> _logger;

        public FileService(
            IBaseReadDL baseReadDL, 
            IBaseWriteDL baseWriteDL, 
            IWebHostEnvironment webHostEnvironment, 
            ILogger<FileService> logger) : base(baseReadDL, baseWriteDL)
        {
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lưu file từ IFormFile và lưu thông tin vào CSDL bảng ResourceFile
        /// </summary>
        public async Task<ResourceFile> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new BadRequestException("Dữ liệu file tải lên không hợp lệ hoặc rỗng.");
            }

            var webRoot = _webHostEnvironment.WebRootPath;
            if (string.IsNullOrEmpty(webRoot))
            {
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            var uploadDir = Path.Combine(webRoot, "upload");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var fileId = Guid.NewGuid();
            var originName = file.FileName;
            var extension = Path.GetExtension(originName);
            var uniqueFileName = $"{fileId}{extension}";
            var physicalPath = Path.Combine(uploadDir, uniqueFileName);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var resourceFile = new ResourceFile
            {
                ResourceFileId = fileId,
                OriginName = originName,
                Type = file.ContentType,
                Size = file.Length,
                Url = $"/upload/{uniqueFileName}",
                State = ModelState.Insert,
                CreatedBy = "System"
            };

            await Save(new List<ResourceFile> { resourceFile });
            _logger.LogInformation("Lưu file thành công. FileId: {FileId}, Url: {Url}", fileId, resourceFile.Url);

            return resourceFile;
        }

        /// <summary>
        /// Lấy link URL của file dựa trên FileId
        /// </summary>
        public async Task<string> GetFileUrlAsync(Guid fileId)
        {
            if (fileId == Guid.Empty)
            {
                throw new ArgumentException("ID file không được để trống.", nameof(fileId));
            }

            var file = await GetFileByIdAsync(fileId);
            if (file == null)
            {
                throw new NotFoundException("Không tìm thấy file tài nguyên tương ứng trong hệ thống.");
            }

            return file.Url;
        }

        /// <summary>
        /// Xóa file vật lý và bản ghi trong CSDL theo FileId
        /// </summary>
        public async Task DeleteFileAsync(Guid fileId)
        {
            if (fileId == Guid.Empty)
            {
                throw new ArgumentException("ID file không được để trống.", nameof(fileId));
            }

            var file = await GetFileByIdAsync(fileId);
            if (file == null)
            {
                _logger.LogWarning("Yêu cầu xóa file với ID {FileId} nhưng bản ghi không tồn tại trong CSDL.", fileId);
                return;
            }

            var webRoot = _webHostEnvironment.WebRootPath;
            if (string.IsNullOrEmpty(webRoot))
            {
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            var fileName = Path.GetFileName(file.Url);
            var physicalPath = Path.Combine(webRoot, "upload", fileName);

            try
            {
                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                    _logger.LogInformation("Đã xóa file vật lý tại: {Path}", physicalPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi xóa file vật lý tại: {Path}", physicalPath);
            }

            file.State = ModelState.Delete;
            await Save(new List<ResourceFile> { file });
            _logger.LogInformation("Đã xóa bản ghi ResourceFile khỏi CSDL cho FileId: {FileId}", fileId);
        }

        /// <summary>
        /// Tìm kiếm ResourceFile theo ID
        /// </summary>
        public async Task<ResourceFile?> GetFileByIdAsync(Guid fileId)
        {
            if (fileId == Guid.Empty)
            {
                throw new ArgumentException("ID file không được để trống.", nameof(fileId));
            }

            var results = await GetByCondition<ResourceFile>(f => f.ResourceFileId == fileId && !f.IsDeleted);
            return results.FirstOrDefault();
        }
    }
}

