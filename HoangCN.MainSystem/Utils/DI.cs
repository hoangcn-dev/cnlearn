using System;
using HoangCN.MainSystem.Interfaces;
using HoangCN.MainSystem.Models;
using HoangCN.MainSystem.Services;
using HoangCN.MainSystem.Utils;
using HoangCN.Core.BL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HoangCN.Core.Common.Utils;

namespace HoangCN.MainSystem.Utils
{
    /// <summary>
    /// Tiện ích đăng ký Dependency Injection cho module HoangCN.MainSystem
    /// </summary>
    public static class DI
    {
        /// <summary>
        /// Đăng ký các dịch vụ của HoangCN.MainSystem
        /// </summary>
        public static IServiceCollection AddMainSystem(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký Core.BL (BaseBL)
            services.AddCoreBL();

            // Đăng ký cấu hình EmailSettings từ section "EmailSettings" kết hợp nạp biến môi trường cho email + mật khẩu
            services.Configure<EmailSettings>(options =>
            {
                configuration.GetSection("EmailSettings").Bind(options);

                // Nạp email và password bắt buộc từ Environment Variables sử dụng EnvKeyUtil
                options.SenderEmail = EnvKeyUtil.GetValue(EnvKeyUtil.HOANGCN_EMAIL_BOT_APP_EMAIL);
                options.Username = EnvKeyUtil.GetValue(EnvKeyUtil.HOANGCN_EMAIL_BOT_APP_EMAIL);
                options.Password = EnvKeyUtil.GetValue(EnvKeyUtil.HOANGCN_EMAIL_BOT_APP_PASSWORD);
            });

            // Đăng ký cấu hình JwtConfig
            services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));

            // Đăng ký công cụ JwtUtil
            services.AddSingleton<JwtUtil>();
            services.AddHttpContextAccessor();

            // Đăng ký dịch vụ IEmailService
            services.AddScoped<IEmailService, EmailService>();

            // Đăng ký dịch vụ quản lý Email Template
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();

            // Đăng ký dịch vụ IFileService chuyên quản lý file tài nguyên
            services.AddScoped<IFileService, FileService>();

            // Đăng ký dịch vụ IRedisService dưới dạng Singleton để chia sẻ kết nối tối ưu
            services.AddSingleton<IRedisService, RedisService>();

            // Đăng ký hosted service để seed mẫu email tự động trên nền async khi app khởi động
            services.AddHostedService<EmailTemplateSeeder>();
            services.AddHostedService<RoleAndUserSeeder>();

            // Đăng ký các Service cho người dùng
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();

            return services;
        }
    }
}
