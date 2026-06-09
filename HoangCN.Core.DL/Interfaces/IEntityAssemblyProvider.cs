using System.Reflection;

namespace HoangCN.Core.DL.Interfaces
{
    /// <summary>
    /// Giao diện cung cấp danh sách các Assembly chứa thực thể nghiệp vụ (Domain Entities) để quét đăng ký động
    /// </summary>
    public interface IEntityAssemblyProvider
    {
        /// <summary>
        /// Lấy danh sách các Assembly chứa thực thể
        /// </summary>
        Assembly[] GetAssemblies();
    }
}
