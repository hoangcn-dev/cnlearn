using System;

namespace HoangCN.Core.BL.Attributes.AuthAction
{
    /// <summary>
    /// Thuộc tính cấu hình phân quyền cho action trong BaseController
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthActionAttribute : Attribute
    {
        /// <summary>
        /// Dùng API này hay không, mặc định là true. Nếu false thì action sẽ bị vô hiệu hóa
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Danh sách các role được phép truy cập vào action này. Nếu null hoặc rỗng thì tất cả các role đều được phép truy cập
        /// </summary>
        public string[]? Roles { get; set; } = null;

        public AuthActionAttribute()
        {
        }
    }
}
