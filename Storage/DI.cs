using Microsoft.Extensions.DependencyInjection;
using Storage.Services;

namespace Storage
{
    public static class DI
    {
        public static IServiceCollection AddStorage(this IServiceCollection services)
        {
            services.AddSingleton<ICloudinaryService, CloudinaryService>();

            return services;
        }
    }
}
