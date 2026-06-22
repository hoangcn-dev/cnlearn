using System;
using System.Linq;
using System.Reflection;

namespace HoangCN.Core.DL.Utils
{
    /// <summary>
    /// Lớp tiện ích cung cấp các cơ chế gọi động phương thức generic/phương thức ảo qua Reflection
    /// </summary>
    public static class DynamicQueryUtil
    {
        /// <summary>
        /// Tìm kiếm, khởi tạo và thực thi một phương thức generic động bằng Reflection
        /// </summary>
        /// <param name="target">Đối tượng đích để thực thi phương thức (null nếu là phương thức static)</param>
        /// <param name="targetType">Kiểu dữ liệu chứa phương thức cần gọi</param>
        /// <param name="methodName">Tên phương thức cần gọi</param>
        /// <param name="genericTypes">Danh sách các kiểu dữ liệu generic truyền vào MakeGenericMethod</param>
        /// <param name="parameters">Danh sách các đối số truyền vào phương thức</param>
        /// <param name="bindingFlags">Cờ chỉ định phạm vi tìm kiếm của phương thức</param>
        /// <returns>Kết quả trả về của phương thức</returns>
        public static object? InvokeGenericMethod(
            object? target,
            Type targetType,
            string methodName,
            Type[] genericTypes,
            object[] parameters,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
        {
            var methods = targetType.GetMethods(bindingFlags)
                .Where(m => m.Name == methodName && m.IsGenericMethod && m.GetGenericArguments().Length == genericTypes.Length);

            // Chọn overload khớp với số lượng tham số truyền vào, nếu không có khớp cụ thể chọn overload đầu tiên
            var method = methods.FirstOrDefault(m => m.GetParameters().Length == parameters.Length)
                ?? methods.FirstOrDefault()
                ?? throw new InvalidOperationException($"Không tìm thấy phương thức generic '{methodName}' trên lớp '{targetType.Name}' với số lượng generic arguments {genericTypes.Length}.");

            var genericMethod = method.MakeGenericMethod(genericTypes);
            return genericMethod.Invoke(target, parameters);
        }
    }
}
