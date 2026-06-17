using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HoangCN.Core.BL.Base;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.MainSystem.Entities;
using HoangCN.Core.DL.Interfaces;
using HoangCN.MainSystem.Interfaces;
using Microsoft.Extensions.Logging;
using HoangCN.Core.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using HoangCN.Core.BL.Utils;

namespace HoangCN.MainSystem.Services
{
    /// <summary>
    /// Triển khai dịch vụ quản lý mẫu email lưu trữ dưới Cơ sở dữ liệu thông qua BaseBL
    /// </summary>
    public class EmailTemplateService : BaseBL<EmailTemplate>, IEmailTemplateService
    {
        private readonly ILogger<EmailTemplateService> _logger;

        public EmailTemplateService(
            IBaseReadDL baseReadDL, 
            IBaseWriteDL baseWriteDL, 
            IHttpContextAccessor httpContextAccessor,
            ILogger<EmailTemplateService> logger) : base(baseReadDL, baseWriteDL, httpContextAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lấy chi tiết mẫu email theo mã code từ cơ sở dữ liệu
        /// </summary>
        public async Task<EmailTemplate> GetTemplateAsync(string templateCode)
        {
            if (string.IsNullOrWhiteSpace(templateCode))
            {
                throw new ArgumentException("TemplateCode cannot be null or empty.", nameof(templateCode));
            }

            var templates = await GetByCondition<EmailTemplate>(t => t.TemplateCode == templateCode);
            var template = templates.FirstOrDefault();

            if (template == null)
            {
                if (templateCode.Equals("forgot_password", StringComparison.OrdinalIgnoreCase))
                {
                    var defaultTemplate = new EmailTemplate
                    {
                        FileResourceId = Guid.NewGuid(),
                        TemplateCode = "forgot_password",
                        Subject = "Yêu cầu khôi phục mật khẩu - HoangCN",
                        Content = "<html><body><h3>Chào {{DisplayName}},</h3><p>Hệ thống đã nhận được yêu cầu khôi phục mật khẩu của bạn.</p><p>Mật khẩu tạm thời mới của bạn là: <strong>{{TemporaryPassword}}</strong> (Hiệu lực trong vòng <strong>{{ExpireTime}}</strong>).</p><p>Vui lòng đăng nhập và tiến hành đổi mật khẩu ngay lập tức để bảo mật thông tin.</p><br/><p>Trân trọng,<br/>Đội ngũ HoangCN</p></body></html>",
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
            ValidateUtil.CommonValidate(new[] { template });

            var allowedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "forgot_password" };
            if (!allowedCodes.Contains(template.TemplateCode))
            {
                throw new BadRequestException($"Kiểu email '{template.TemplateCode}' không được hỗ trợ trong hệ thống.");
            }

            if (template.TemplateCode.Equals("forgot_password", StringComparison.OrdinalIgnoreCase))
            {
                if (!template.Content.Contains("{{DisplayName}}") || !template.Content.Contains("{{TemporaryPassword}}"))
                {
                    throw new BadRequestException("Mẫu email quên mật khẩu không được thiếu các tham số bắt buộc: {{DisplayName}} hoặc {{TemporaryPassword}}.");
                }
            }

            var existingTemplates = await GetByCondition<EmailTemplate>(t => t.TemplateCode == template.TemplateCode);
            var existing = existingTemplates.FirstOrDefault();
            
            if (existing != null)
            {
                existing.Subject = template.Subject.Trim();
                existing.Content = template.Content.Trim();
                existing.ModifiedBy = "Admin";
                
                _logger.LogInformation("Saving email template '{TemplateCode}' to database", existing.TemplateCode);
                await UpdateAsync(new List<EmailTemplate> { existing });
            }
            else
            {
                if (template.FileResourceId == Guid.Empty)
                {
                    template.FileResourceId = Guid.NewGuid();
                }
                template.CreatedBy = "System";
                
                _logger.LogInformation("Saving email template '{TemplateCode}' to database", template.TemplateCode);
                await InsertAsync(new List<EmailTemplate> { template });
            }
        }

        /// <summary>
        /// Lấy tất cả mẫu email hiện tại từ cơ sở dữ liệu
        /// </summary>
        public async Task<List<EmailTemplate>> GetAllTemplatesAsync()
        {
            var templates = await GetByCondition<EmailTemplate>(t => !t.IsDeleted);

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

