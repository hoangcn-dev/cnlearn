using HoangCN.Core.Common.Utils;
using HoangCN.LearnMS.Entities;
using HoangCN.Core.DL;
using HoangCN.LearnMS.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace HoangCN.LearnMS
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

            // Đăng ký Core.DL cho LearnMS sử dụng database hoangcn_learn_write và hoangcn_learn_read
            var writeConnectionString = builder.Configuration.GetConnectionString("LearnWrite");
            var readConnectionString = builder.Configuration.GetConnectionString("LearnRead");

            if (string.IsNullOrEmpty(writeConnectionString))
            {
                throw new InvalidOperationException("Connection string 'LearnWrite' is null or empty.");
            }
            if (string.IsNullOrEmpty(readConnectionString))
            {
                throw new InvalidOperationException("Connection string 'LearnRead' is null or empty.");
            }

            builder.Services.AddCoreDL(
                writeConnectionString: writeConnectionString,
                readConnectionString: readConnectionString,
                migrationsAssembly: "HoangCN.LearnMS",
                scanAssemblies: new[] { 
                    typeof(Question).Assembly // Quét các thực thể của LearnMS (Question, QuestionCategory, v.v.)
                }
            );

            // Đăng ký các dịch vụ của LearnMS
            builder.Services.AddLearnMS(builder.Configuration);

            var app = builder.BuildCommon(config);

            // Tự động chạy Migration khi ứng dụng khởi động để tạo/cập nhật bảng trong database hoangcn_learn_write
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DynamicDbContext>();
                dbContext.Database.Migrate();
            }

            app.Run();
        }
    }
}
