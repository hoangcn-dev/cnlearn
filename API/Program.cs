using API.Middlewares;
using Asp.Versioning;
using Core.Utilities;
using DataAccess;
using Module.Users;
using Module.Users.Entities;
using Module.MediaDownloader;
using Storage;
using System.Security.Claims;
using SystemTracking;
using BackgroundTask;
using Realtime;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;
            var services = builder.Services;

            // Authorize
            services.AddAuthorization(options =>
            {
               options.AddPolicy(StringUtil.PolicyNames.OnlyAdmin, p =>
               {
                  p.RequireClaim(ClaimTypes.Role, Roles.ADMIN); 
               });
            });

            // Default
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Add API versioning
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),              // /api/v1/users
                    new QueryStringApiVersionReader("api-version"),     // ?api-version=1.0
                    new HeaderApiVersionReader("x-api-version")       // x-api-version: 1.0
                );
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // Custom
            services.AddTransient<ExceptionMiddleware>();
            services.AddTransient<TokenMiddleware>();
            services.AddRouting(opt => opt.LowercaseUrls = true);
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocal",
                    builder => builder
                        .WithOrigins("http://localhost:4200")
                        .AllowCredentials()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
                options.AddPolicy("AllowTool",
                    builder => builder
                        .WithOrigins("https://tool.hoangcn.com")
                        .AllowCredentials()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            services.AddPostgreSQL();
            services.AddRepository();
            services.AddSystemService(configuration);
            services.AddStorage();
            services.AddBackgroundTask();
            services.AddRealtimeService();

            services.AddUserModule(configuration);
            services.AddMediaDownloadModule(configuration);

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<TokenMiddleware>();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    // Tự động tạo các endpoint cho từng version
                    var versions = app.DescribeApiVersions();
                    foreach (var version in versions)
                    {
                        var groupName = version.GroupName;
                        options.SwaggerEndpoint(
                            $"/swagger/{groupName}/swagger.json",
                            $"API {groupName.ToUpperInvariant()}");
                    }

                    options.RoutePrefix = "swagger";
                });
            }
            app.UseHttpsRedirection();
            app.UseCors("AllowLocal");
            app.UseCors("AllowTool");

            app.UseBackgroundTask();
            app.UseMediaDownloadModule();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();
            await app.InitializeDatabaseAsync();
            app.RunMediaDownloaderBackgroundTask();
            app.Run();
        }
    }
}
