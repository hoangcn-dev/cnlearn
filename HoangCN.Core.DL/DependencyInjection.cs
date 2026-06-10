using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using HoangCN.Core.DL.Interfaces;
using HoangCN.Core.DL.Implementation;
using System.Reflection;

namespace HoangCN.Core.DL
{
    /// <summary>
    /// Các phương thức mở rộng để cấu hình và tiêm phụ thuộc các dịch vụ dữ liệu
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Đăng ký các dịch vụ dữ liệu lõi Core.DL (EF Core cho Write, Dapper cho Read)
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="writeConnectionString">Chuỗi kết nối database ghi (EF Core)</param>
        /// <param name="readConnectionString">Chuỗi kết nối database đọc (Dapper). Nếu rỗng sẽ dùng chung với database ghi.</param>
        /// <param name="migrationsAssembly">Tên Assembly chứa Migration (Ví dụ: "HoangCN.LearnMS.Infrastructure")</param>
        /// <param name="scanAssemblies">Danh sách các Assembly chứa thực thể cần đăng ký.</param>
        /// <returns></returns>
        public static IServiceCollection AddCoreDL(
            this IServiceCollection services, 
            string writeConnectionString, 
            string? readConnectionString = null,
            string? migrationsAssembly = null,
            Assembly[]? scanAssemblies = null)
        {
            var finalReadConnectionString = string.IsNullOrWhiteSpace(readConnectionString) 
                ? writeConnectionString 
                : readConnectionString;

            // Đăng ký EntityAssemblyProvider nếu có truyền danh sách Assembly quét thực thể
            if (scanAssemblies != null && scanAssemblies.Length > 0)
            {
                services.AddSingleton<IEntityAssemblyProvider>(new EntityAssemblyProvider(scanAssemblies));
            }

            // 1. Đăng ký DbContext (Write DB) - Chỉ định MigrationsAssembly động
            services.AddDbContext<DynamicDbContext>(options =>
            {
                options.UseMySQL(writeConnectionString, b =>
                {
                    if (!string.IsNullOrEmpty(migrationsAssembly))
                    {
                        b.MigrationsAssembly(migrationsAssembly);
                    }
                });
            });

            // Tiêm DbContext dùng chung cho các repository cần tham chiếu DbContext
            services.AddScoped<DbContext>(sp => sp.GetRequiredService<DynamicDbContext>());

            // 2. Đăng ký cơ chế Đọc (IBaseReadDL sử dụng Dapper)
            services.AddScoped<IBaseReadDL>(sp => new BaseReadDL(finalReadConnectionString));

            // 3. Đăng ký cơ chế Ghi (IBaseWriteDL sử dụng EF Core)
            services.AddScoped<IBaseWriteDL, BaseWriteDL>();

            return services;
        }
    }
}
