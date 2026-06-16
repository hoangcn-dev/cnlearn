using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HoangCN.Core.DL
{
    /// <summary>
    /// Công cụ hỗ trợ cập nhật cấu trúc cơ sở dữ liệu động trong quá trình phát triển
    /// </summary>
    public static class MigrationHelper
    {
        /// <summary>
        /// Tự động quét và thực thi các migration chưa được áp dụng trên database ghi
        /// </summary>
        /// <param name="serviceProvider">Bộ cung cấp dịch vụ của ứng dụng</param>
        public static void ApplyMigrations(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DynamicDbContext>();
                
                // EF Core thực hiện tự động tạo DB và Apply các file migration hiện có
                context.Database.Migrate();
            }
        }
    }
}
