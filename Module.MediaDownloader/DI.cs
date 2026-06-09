using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.MediaDownloader.Configurations;
using Module.MediaDownloader.Services;

namespace Module.MediaDownloader
{
    public static class DI
    {
        public static IServiceCollection AddMediaDownloadModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IYtDlpService, YtDlpService>();
            return services;
        }
    }
}
