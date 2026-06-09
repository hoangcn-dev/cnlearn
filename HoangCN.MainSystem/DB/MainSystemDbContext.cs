using HoangCN.Core.DL;
using HoangCN.Core.DL.Implementation;
using HoangCN.MainSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HoangCN.MainSystem.DB
{
    /// <summary>
    /// Factory cung cấp DynamicDbContext tại thời điểm Design-time để phục vụ việc tạo migration độc lập cho MainSystem
    /// </summary>
    public class DynamicDbContextFactory : IDesignTimeDbContextFactory<DynamicDbContext>
    {
        public DynamicDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DynamicDbContext>();
            optionsBuilder.UseMySQL(
                "Server=localhost;Port=3306;Database=hoangcn_mainsys;Uid=root;Pwd=admin;", 
                b => b.MigrationsAssembly("HoangCN.MainSystem")
            );

            // Chỉ định quét chính xác thực thể của MainSystem khi chạy Migration tại đây
            var provider = new EntityAssemblyProvider(new[] { typeof(User).Assembly });

            return new DynamicDbContext(optionsBuilder.Options, provider);
        }
    }
}
