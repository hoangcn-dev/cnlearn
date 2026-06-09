using HoangCN.Common.Utils;
using HoangCN.MainSystem.Enums;
using HoangCN.MainSystem.Utils;
using HoangCN.Core.DL;
using HoangCN.MainSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HoangCN.MainSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<List<string>>();
            if (allowedOrigins == null || allowedOrigins.Count == 0)
            {
                allowedOrigins = new List<string> { "http://localhost:5173", "https://localhost:5173" };
            }
            var config = new ConfigCommonOptions
            {
                CorOrigins = allowedOrigins
            };

            builder.ConfigCommon(config);

            // Đăng ký Core.DL thế hệ mới (EF Core Write, Dapper Read) cho MainSystem
            var connectionString = builder.Configuration.GetConnectionString("MySQL");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'MySQL' is null or empty.");
            }

            builder.Services.AddCoreDL(
                writeConnectionString: connectionString,
                readConnectionString: connectionString,
                migrationsAssembly: "HoangCN.MainSystem",
                scanAssemblies: new[] { 
                    typeof(User).Assembly          // Chỉ quét thực thể của MainSystem (User, Role, EmailTemplate, ResourceFile)
                }
            );

            // Đăng ký các dịch vụ của MainSystem (bao gồm UserManagement & System Services)
            builder.Services.AddMainSystem(builder.Configuration);

            // Đăng ký Policy phân quyền Admin
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole(RoleNames.Admin.ToString()));
            });

            var app = builder.BuildCommon(config);

            // Tự động chạy Migration khi ứng dụng khởi động để tạo bảng/cột mới trong database
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DynamicDbContext>();
                dbContext.Database.Migrate();
            }

            app.Run();
        }
    }
}
