using HoangCN.DL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MISA.CukCuk.DL.Base;

namespace MISA.CukCuk.DL.Extensions
{
    /// <summary>
    /// Lớp cấu hình Dependency Inject cho tầng nghiệp vụ
    /// </summary>
    public static class DI
    {
        public static IServiceCollection AddDL(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MySQL");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Chưa cấu hình chuỗi kết nối cho MySQL hoặc chuỗi kết nối trống");
            }
            services.AddScoped<IBaseDL, BaseDL>(sp => new BaseDL(connectionString));

            // Cấu hình Mapping từ CamelCase sang snake_case
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            return services;
        }
    }
}
