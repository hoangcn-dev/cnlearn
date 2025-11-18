using Core.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccess
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(EnvUtil.GetEnv(EnvUtil.Keys.HOANGCN_PGDB_CONNECTION_STRING));

            var context = new AppDbContext(optionsBuilder.Options);
            return context;
        }
    }
}
