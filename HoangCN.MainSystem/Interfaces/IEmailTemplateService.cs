using System.Collections.Generic;
using System.Threading.Tasks;
using HoangCN.Common.Model.Entities;
using HoangCN.BL.Interfaces;

namespace HoangCN.MainSystem.Interfaces
{
    /// <summary>
    /// Giao diện dịch vụ quản lý mẫu email (Template Email)
    /// </summary>
    public interface IEmailTemplateService : IBaseBL<EmailTemplate>
    {
        /// <summary>
        /// Lấy thông tin mẫu email dựa trên mã code
        /// </summary>
        /// <param name="templateCode">Mã mẫu email</param>
        /// <returns>Thông tin mẫu email</returns>
        Task<EmailTemplate> GetTemplateAsync(string templateCode);

        /// <summary>
        /// Lưu hoặc cập nhật mẫu email
        /// </summary>
        /// <param name="template">Đối tượng mẫu email cần lưu</param>
        /// <returns></returns>
        Task SaveTemplateAsync(EmailTemplate template);

        /// <summary>
        /// Lấy tất cả danh sách các mẫu email hiện có
        /// </summary>
        /// <returns>Danh sách mẫu email</returns>
        Task<List<EmailTemplate>> GetAllTemplatesAsync();
    }
}
