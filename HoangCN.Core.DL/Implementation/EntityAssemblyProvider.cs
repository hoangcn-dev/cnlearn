using HoangCN.Core.DL.Interfaces;
using System.Reflection;

namespace HoangCN.Core.DL.Implementation
{
    /// <summary>
    /// Triển khai cung cấp danh sách Assembly chứa thực thể
    /// </summary>
    public class EntityAssemblyProvider : IEntityAssemblyProvider
    {
        private readonly Assembly[] _assemblies;

        public EntityAssemblyProvider(params Assembly[] assemblies)
        {
            _assemblies = assemblies ?? Array.Empty<Assembly>();
        }

        public Assembly[] GetAssemblies()
        {
            return _assemblies;
        }
    }
}
