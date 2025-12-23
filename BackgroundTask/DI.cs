using Core.Utilities;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BackgroundTask
{
    public static class DI
    {
        public static IServiceCollection AddBackgroundTask(this IServiceCollection services)
        {
            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
                config.UseSimpleAssemblyNameTypeSerializer();
                config.UseRecommendedSerializerSettings();
                config.UseStorage(new PostgreSqlStorage(
                    EnvUtil.GetEnv(EnvUtil.Keys.HOANGCN_PGDB_FOR_HANGFIRE_CONNECTION_STRING),
                    new PostgreSqlStorageOptions
                    {
                        SchemaName = "Hangfire",
                        PrepareSchemaIfNecessary = true,
                        QueuePollInterval = TimeSpan.FromSeconds(15),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        UseNativeDatabaseTransactions = true,
                        InvisibilityTimeout = TimeSpan.FromMinutes(30)
                    }));
            });
            services.AddHangfireServer();

            return services;
        }

        public static WebApplication UseBackgroundTask(this WebApplication app)
        {
            app.UseHangfireDashboard();
            return app;
        }
    }
}
