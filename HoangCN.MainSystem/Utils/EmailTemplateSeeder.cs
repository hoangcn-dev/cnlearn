using HoangCN.MainSystem.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HoangCN.MainSystem.Utils
{
    /// <summary>
    /// Seeder tự động nạp mẫu email mặc định vào cơ sở dữ liệu khi hệ thống khởi chạy
    /// </summary>
    public class EmailTemplateSeeder : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public EmailTemplateSeeder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Kích hoạt khi ứng dụng web khởi động
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var emailTemplateService = scope.ServiceProvider.GetService<IEmailTemplateService>();
                if (emailTemplateService != null)
                {
                    try
                    {
                        // Sử dụng từ khóa await để gọi GetTemplateAsync("forgot_password") tự động khởi tạo seeding
                        await emailTemplateService.GetTemplateAsync("forgot_password");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[EmailTemplateSeeder] Lỗi khi seed email template mặc định: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Kích hoạt khi ứng dụng web tắt
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
