using HoangCN.UserManagement.Interfaces;
using HoangCN.UserManagement.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZstdSharp.Unsafe;

namespace HoangCN.UserManagement.Utils
{
    /// <summary>
    /// Tiện ích đăng ký Dependency Injection cho module HoangCN.UserManagement
    /// </summary>
    public static class DI
    {
        public static IServiceCollection AddUserManagement(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký cấu hình JwtConfig
            services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));

            // Đăng ký công cụ JwtUtil
            services.AddSingleton<JwtUtil>();
            services.AddHttpContextAccessor();

            // Đăng ký các Service
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();

            return services;
        }
    }
}
