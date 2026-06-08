
using HoangCN.BL.Utils;
using HoangCN.Common.Utils;
using HoangCN.UserManagement.Enums;
using HoangCN.UserManagement.Utils;
using HoangCN.MainSystem.Utils;
using HoangCN.LearnMS.Utils;

namespace HoangCN.API
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
            builder.Services.AddBL(builder.Configuration);
            builder.Services.AddUserManagement(builder.Configuration);
            builder.Services.AddMainSystem(builder.Configuration);
            builder.Services.AddLearnMS(builder.Configuration);

            // Đăng ký Policy phân quyền Admin
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole(RoleNames.Admin.ToString()));
            });

            var app = builder.BuildCommon(config);
            app.Run();
        }
    }
}
