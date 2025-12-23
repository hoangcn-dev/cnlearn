using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace SystemTracking
{
    public static class DI
    {
        public static IServiceCollection AddSystemService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSerilog((context, logger) =>
            {
                var logPath = Path.Combine(AppContext.BaseDirectory, "logs", ".log");

                logger
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.File(
                        logPath,
                        rollingInterval: RollingInterval.Day,
                        outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"
                    );
            });

            return services;
        }
    }
}
