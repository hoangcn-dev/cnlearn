using HoangCN.Common.Base;
using HoangCN.Core.DL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HoangCN.Core.DL
{
    /// <summary>
    /// DbContext động, tự động quét các thực thể từ các Assembly chỉ định
    /// </summary>
    public class DynamicDbContext : DbContext
    {
        private readonly Assembly[] _scanAssemblies;

        public DynamicDbContext(
            DbContextOptions<DynamicDbContext> options, 
            IEntityAssemblyProvider? assemblyProvider = null)
            : base(options)
        {
            // Lấy danh sách Assembly từ Provider được tiêm vào hoặc mặc định là HoangCN.Common
            _scanAssemblies = assemblyProvider?.GetAssemblies() 
                ?? new[] { typeof(BaseEntity).Assembly };
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var assembly in _scanAssemblies)
            {
                // Tìm kiếm tất cả các class kế thừa từ BaseEntity và không phải class abstract
                var entityTypes = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseEntity)));

                foreach (var type in entityTypes)
                {
                    modelBuilder.Entity(type);
                }
            }
        }
    }
}
