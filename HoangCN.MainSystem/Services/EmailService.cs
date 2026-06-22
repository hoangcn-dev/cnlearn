using HoangCN.MainSystem.Interfaces;
using HoangCN.MainSystem.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HoangCN.MainSystem.Services
{
    /// <summary>
    /// Triển khai dịch vụ gửi email bằng thư viện MailKit qua SMTP
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            // Lấy cấu hình email từ IOptions
            var settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            
            // Kiểm tra các trường cấu hình bắt buộc theo quy tắc:
            // "No redundant fallbacks, use standard cases only => Throw an exception if null or empty."
            if (string.IsNullOrWhiteSpace(settings.SmtpServer))
            {
                throw new ArgumentException("SmtpServer is null or empty. SmtpServer must be configured.", nameof(options));
            }
            if (settings.SmtpPort <= 0)
            {
                throw new ArgumentException("SmtpPort must be a positive integer.", nameof(options));
            }
            if (string.IsNullOrWhiteSpace(settings.SenderEmail))
            {
                throw new ArgumentException("SenderEmail is null or empty. SenderEmail must be configured.", nameof(options));
            }
            if (string.IsNullOrWhiteSpace(settings.Username))
            {
                throw new ArgumentException("Username is null or empty. Username must be configured.", nameof(options));
            }
            if (string.IsNullOrWhiteSpace(settings.Password))
            {
                throw new ArgumentException("Password is null or empty. Password must be configured.", nameof(options));
            }

            _settings = settings;
        }

        /// <summary>
        /// Gửi email bất đồng bộ thông thường
        /// </summary>
        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            await SendMailInternalAsync(toEmail, subject, body, null, isHtml);
        }

        /// <summary>
        /// Gửi email bất đồng bộ kèm tệp đính kèm
        /// </summary>
        public async Task SendEmailWithAttachmentsAsync(
            string toEmail, 
            string subject, 
            string body, 
            IEnumerable<(string FileName, byte[] Content)> attachments, 
            bool isHtml = true)
        {
            await SendMailInternalAsync(toEmail, subject, body, attachments, isHtml);
        }

        /// <summary>
        /// Xử lý gửi email chung (bao gồm cấu hình SMTP và MimeMessage)
        /// </summary>
        private async Task SendMailInternalAsync(
            string toEmail, 
            string subject, 
            string body, 
            IEnumerable<(string FileName, byte[] Content)>? attachments, 
            bool isHtml)
        {
            // Kiểm tra tính hợp lệ của tham số đầu vào
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                throw new ArgumentException("Recipient email (toEmail) cannot be null or empty.", nameof(toEmail));
            }
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("Email subject cannot be null or empty.", nameof(subject));
            }
            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentException("Email body cannot be null or empty.", nameof(body));
            }

            var message = new MimeMessage();
            
            // Thiết lập thông tin người gửi
            var senderName = string.IsNullOrWhiteSpace(_settings.SenderName) ? _settings.SenderEmail : _settings.SenderName;
            message.From.Add(new MailboxAddress(senderName, _settings.SenderEmail));
            
            // Thiết lập thông tin người nhận
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            // Xây dựng nội dung email
            var builder = new BodyBuilder();
            if (isHtml)
            {
                builder.HtmlBody = body;
            }
            else
            {
                builder.TextBody = body;
            }

            // Xử lý tệp đính kèm
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    if (string.IsNullOrWhiteSpace(attachment.FileName))
                    {
                        throw new ArgumentException("Attachment filename cannot be null or empty.", nameof(attachments));
                    }
                    if (attachment.Content == null || attachment.Content.Length == 0)
                    {
                        throw new ArgumentException($"Attachment '{attachment.FileName}' content is null or empty.", nameof(attachments));
                    }
                    
                    builder.Attachments.Add(attachment.FileName, attachment.Content);
                }
            }

            message.Body = builder.ToMessageBody();

            // Kết nối và gửi qua SMTP
            using var client = new SmtpClient();
            try
            {
                // Tự động xác định chế độ bảo mật dựa trên cổng kết nối
                var secureSocketOptions = SecureSocketOptions.Auto;
                if (_settings.SmtpPort == 465)
                {
                    secureSocketOptions = SecureSocketOptions.SslOnConnect; // Implicit SSL/TLS
                }
                else if (_settings.SmtpPort == 587)
                {
                    secureSocketOptions = SecureSocketOptions.StartTls; // Explicit TLS
                }
                else
                {
                    secureSocketOptions = _settings.EnableSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable;
                }

                _logger.LogInformation("Connecting to SMTP server {Server}:{Port}...", _settings.SmtpServer, _settings.SmtpPort);
                await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, secureSocketOptions);

                _logger.LogInformation("Authenticating SMTP user {Username}...", _settings.Username);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);

                _logger.LogInformation("Sending email to {To}...", toEmail);
                await client.SendAsync(message);
                
                _logger.LogInformation("Email sent successfully to {To}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}. Error: {Message}", toEmail, ex.Message);
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
