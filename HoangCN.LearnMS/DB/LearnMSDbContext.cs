using HoangCN.LearnMS.Entities;
using HoangCN.Core.DL;
using HoangCN.Core.DL.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using HoangCN.Core.Common.Utils;
using HoangCN.LearnMS.Utils;

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
                EnvUtil.GetValue(EnvKeys.CONNNECTION_STRING_MYSQL_LEARN_WRITE), 
                b => b.MigrationsAssembly("HoangCN.LearnMS")
            );

            // Chỉ định quét chính xác thực thể của LearnMS khi chạy Migration
            var provider = new EntityAssemblyProvider([typeof(Question).Assembly]);

            return new DynamicDbContext(optionsBuilder.Options, provider);
        }
    }
}
