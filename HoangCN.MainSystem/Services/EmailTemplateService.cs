using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HoangCN.BL.Base;
using HoangCN.Common.Base;
using HoangCN.Common.Enums;
using HoangCN.Common.Model.Entities;
using HoangCN.DL.Interfaces;
using HoangCN.MainSystem.Interfaces;
using Microsoft.Extensions.Logging;
using HoangCN.Common.Exceptions;

namespace HoangCN.MainSystem.Services
{
    /// <summary>
    /// Triển khai dịch vụ quản lý mẫu email lưu trữ dưới Cơ sở dữ liệu thông qua BaseBL
    /// </summary>
    public class EmailTemplateService : BaseBL<EmailTemplate>, IEmailTemplateService
    {
        private readonly ILogger<EmailTemplateService> _logger;

        public EmailTemplateService(IBaseDL baseDL, ILogger<EmailTemplateService> logger) : base(baseDL)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lấy chi tiết mẫu email theo mã code từ cơ sở dữ liệu
        /// </summary>
        public async Task<EmailTemplate> GetTemplateAsync(string templateCode)
        {
            // Kiểm tra tính hợp lệ của tham số theo quy tắc Rule 1:
            // "No redundant fallbacks, use standard cases only => Throw an exception if null or empty."
            if (string.IsNullOrWhiteSpace(templateCode))
            {
                throw new ArgumentException("TemplateCode cannot be null or empty.", nameof(templateCode));
            }

            // Truy vấn qua BaseBL (GetByCondition) - Không viết SQL tùy chỉnh (tuân thủ Rule 6)
            var templates = await GetByCondition<EmailTemplate>(t => t.TemplateCode == templateCode);
            var template = templates.FirstOrDefault();

            if (template == null)
            {
                // Nếu chưa tồn tại template quên mật khẩu mặc định, tiến hành khởi tạo seeding tự động vào DB
                if (templateCode.Equals("forgot_password", StringComparison.OrdinalIgnoreCase))
                {
                    var defaultTemplate = new EmailTemplate
                    {
                        FileResourceId = Guid.NewGuid(),
                        TemplateCode = "forgot_password",
                        Subject = "Yêu cầu khôi phục mật khẩu - HoangCN",
                        Content = "<html><body><h3>Chào {{DisplayName}},</h3><p>Hệ thống đã nhận được yêu cầu khôi phục mật khẩu của bạn.</p><p>Mật khẩu tạm thời mới của bạn là: <strong>{{TemporaryPassword}}</strong> (Hiệu lực trong vòng <strong>{{ExpireTime}}</strong>).</p><p>Vui lòng đăng nhập và tiến hành đổi mật khẩu ngay lập tức để bảo mật thông tin.</p><br/><p>Trân trọng,<br/>Đội ngũ HoangCN</p></body></html>",
                        State = ModelState.Insert,
                        CreatedBy = "System"
                    };
                    await SaveTemplateAsync(defaultTemplate);
                    return defaultTemplate;
                }
                
                throw new KeyNotFoundException($"Email template with code '{templateCode}' was not found.");
            }

            return template;
        }

        /// <summary>
        /// Lưu hoặc cập nhật mẫu email vào cơ sở dữ liệu
        /// </summary>
        public async Task SaveTemplateAsync(EmailTemplate template)
        {
            // Kiểm tra hợp lệ dữ liệu đầu vào theo Rule 1
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }
            if (string.IsNullOrWhiteSpace(template.TemplateCode))
            {
                throw new BadRequestException("Mã template không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(template.Subject))
            {
                throw new BadRequestException("Tiêu đề email không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(template.Content))
            {
                throw new BadRequestException("Nội dung template không được để trống.");
            }

            // 1. Chỉ chấp nhận các mã code email cố định (Hiện tại chỉ có forgot_password)
            var allowedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "forgot_password" };
            if (!allowedCodes.Contains(template.TemplateCode))
            {
                throw new BadRequestException($"Kiểu email '{template.TemplateCode}' không được hỗ trợ trong hệ thống.");
            }

            // 2. Mỗi template có các tham số mặc định không được thiếu
            if (template.TemplateCode.Equals("forgot_password", StringComparison.OrdinalIgnoreCase))
            {
                if (!template.Content.Contains("{{DisplayName}}") || !template.Content.Contains("{{TemporaryPassword}}"))
                {
                    throw new BadRequestException("Mẫu email quên mật khẩu không được thiếu các tham số bắt buộc: {{DisplayName}} hoặc {{TemporaryPassword}}.");
                }
            }

            // 3. Chỉ cho phép chỉnh sửa mẫu email đã tồn tại, không cho phép thêm mới qua API
            var existingTemplates = await GetByCondition<EmailTemplate>(t => t.TemplateCode == template.TemplateCode);
            var existing = existingTemplates.FirstOrDefault();
            
            if (existing != null)
            {
                existing.Subject = template.Subject.Trim();
                existing.Content = template.Content.Trim();
                existing.State = ModelState.Update;
                existing.ModifiedBy = "Admin";
                existing.ModifiedDate = DateTime.Now;
                
                _logger.LogInformation("Saving email template '{TemplateCode}' to database", existing.TemplateCode);
                await Save(new List<EmailTemplate> { existing });
            }
            else
            {
                if (template.FileResourceId == Guid.Empty)
                {
                    template.FileResourceId = Guid.NewGuid();
                }
                template.State = ModelState.Insert;
                template.CreatedBy = "System";
                template.CreatedDate = DateTime.Now;
                
                _logger.LogInformation("Saving email template '{TemplateCode}' to database", template.TemplateCode);
                await Save(new List<EmailTemplate> { template });
            }
        }

        /// <summary>
        /// Lấy tất cả mẫu email hiện tại từ cơ sở dữ liệu
        /// </summary>
        public async Task<List<EmailTemplate>> GetAllTemplatesAsync()
        {
            // Lấy các bản ghi chưa bị xóa (IsDeleted == false) bằng GetByCondition của BaseBL
            var templates = await GetByCondition<EmailTemplate>(t => !t.IsDeleted);

            // Đảm bảo luôn có ít nhất template quên mật khẩu mặc định (seeding nếu trống)
            if (templates == null || templates.Count == 0)
            {
                templates = new List<EmailTemplate>();
                try
                {
                    var defaultForgotPassword = await GetTemplateAsync("forgot_password");
                    templates.Add(defaultForgotPassword);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to seed default forgot_password template to database.");
                }
            }

            return templates;
        }
    }
}
