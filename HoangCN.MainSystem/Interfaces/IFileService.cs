using HoangCN.Common.Model.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace HoangCN.MainSystem.Interfaces
{
    /// <summary>
    /// Giao diện quản lý lưu trữ, truy vấn và xóa file tài nguyên
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Lưu file từ IFormFile vào thư mục upload vật lý và tạo đối tượng ResourceFile tương ứng trong CSDL
        /// </summary>
        /// <param name="file">File được tải lên từ request</param>
        /// <returns>Đối tượng ResourceFile chứa thông tin file đã lưu</returns>
        Task<ResourceFile> SaveFileAsync(IFormFile file);

        /// <summary>
        /// Trả về link file (URL) từ FileId
        /// </summary>
        /// <param name="fileId">ID của file tài nguyên</param>
        /// <returns>Đường dẫn Url dạng chuỗi</returns>
        Task<string> GetFileUrlAsync(Guid fileId);

        /// <summary>
        /// Xóa file vật lý khỏi đĩa và xóa bản ghi khỏi CSDL theo FileId
        /// </summary>
        /// <param name="fileId">ID của file tài nguyên cần xóa</param>
        Task DeleteFileAsync(Guid fileId);

        /// <summary>
        /// Lấy chi tiết thông tin đối tượng ResourceFile theo ID
        /// </summary>
        /// <param name="fileId">ID của file tài nguyên</param>
        /// <returns>ResourceFile hoặc null nếu không tồn tại</returns>
        Task<ResourceFile?> GetFileByIdAsync(Guid fileId);
    }
}
