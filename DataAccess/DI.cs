using Core.Interfaces;
using Core.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Emit;

namespace DataAccess
{
    public static class DI
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddPostgreSQL(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(EnvUtil.GetEnv(EnvUtil.Keys.HOANGCN_PGDB_CONNECTION_STRING));
            });
            services.AddScoped<Initializer>();

            return services;
        }

        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<Initializer>();
            await initializer.CheckAndUpdateFromMigrationAsync();
            await initializer.CheckAndInitDefaultDataAsync();
        }
    }
}
