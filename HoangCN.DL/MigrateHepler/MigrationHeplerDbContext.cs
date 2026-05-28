using HoangCN.Common.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HoangCN.DL.MigrateHepler
{
    public class MigrationHeplerDbContext : DbContext
    {
        public MigrationHeplerDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tự động detect các Base Entity
            var entityTypes = typeof(BaseEntity).Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseEntity)));
            foreach (var type in entityTypes)
            {
                modelBuilder.Entity(type);
            }
        }
    }

    public class MigrationHeplerDbContextFactory : IDesignTimeDbContextFactory<MigrationHeplerDbContext>
    {
        public MigrationHeplerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MigrationHeplerDbContext>();
            optionsBuilder.UseMySQL("Server=localhost;Port=3306;Database=hoangcn;Uid=root;Pwd=admin;");

            var context = new MigrationHeplerDbContext(optionsBuilder.Options);
            return context;
        }
    }
}
