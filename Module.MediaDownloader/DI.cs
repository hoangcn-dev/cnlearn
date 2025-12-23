using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.MediaDownloader.Configurations;
using Module.MediaDownloader.Hubs;
using Module.MediaDownloader.Services;

namespace Module.MediaDownloader
{
    public static class DI
    {
        public static IServiceCollection AddMediaDownloadModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StorageConfiguration>(configuration.GetSection("Cloudinary"));

            services.AddTransient<IDownloadMediaService, DownloadMediaService>();
            services.AddSingleton<IManageProgressService, ManageProgressService>();
            services.AddSingleton<IYtDlpService, YtDlpService>();

            return services;
        }

        public static WebApplication UseMediaDownloadModule(this  WebApplication app)
        {
            app.MapHub<ProgressHub>(ProgressHub.HUB_URL);
            return app;
        }

        public static void RunMediaDownloaderBackgroundTask(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            RecurringJob.AddOrUpdate<ManageProgressService>(
                "media_file_temps_clean",
                s => s.CleanAsync(),
                Cron.Minutely);
        }
    }
}
