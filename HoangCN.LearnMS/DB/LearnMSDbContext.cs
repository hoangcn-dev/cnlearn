using HoangCN.LearnMS.Entities;
using HoangCN.Core.DL;
using HoangCN.Core.DL.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HoangCN.LearnMS.DB
{
    /// <summary>
    /// Factory cung cấp DynamicDbContext tại thời điểm Design-time để phục vụ việc tạo migration độc lập cho LearnMS
    /// </summary>
    public class LearnMSDbContextFactory : IDesignTimeDbContextFactory<DynamicDbContext>
    {
        public DynamicDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DynamicDbContext>();
            optionsBuilder.UseMySQL(
                "Server=localhost;Port=3306;Database=hoangcn_learn_write;Uid=root;Pwd=admin;", 
                b => b.MigrationsAssembly("HoangCN.LearnMS")
            );

            // Chỉ định quét chính xác thực thể của LearnMS khi chạy Migration
            var provider = new EntityAssemblyProvider(new[] { typeof(Question).Assembly });

            return new DynamicDbContext(optionsBuilder.Options, provider);
        }
    }
}
