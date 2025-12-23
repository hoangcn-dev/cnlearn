using Microsoft.Extensions.DependencyInjection;

namespace Realtime
{
    public static class DI
    {
        public static IServiceCollection AddRealtimeService(this IServiceCollection services)
        {
            services.AddSignalR();

            return services;
        }
    }
}
