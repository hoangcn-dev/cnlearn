using System.Collections.Generic;

namespace HoangCN.Core.BL.Base
{
    /// <summary>
    /// Bộ dựng cấu hình phân quyền (Fluent API) cho các Action trong BaseController
    /// </summary>
    public class AuthActionPolicyBuilder
    {
        private readonly Dictionary<string, AuthActionSettings> _policies;

        public AuthActionPolicyBuilder(Dictionary<string, AuthActionSettings> policies)
        {
            _policies = policies;
        }

        /// <summary>
        /// Bảo vệ action: Yêu cầu đăng nhập và có vai trò cụ thể (nếu có truyền vào)
        /// </summary>
        /// <param name="actionName">Tên action (Ví dụ: nameof(Insert))</param>
        /// <param name="roles">Danh sách các vai trò được phép truy cập</param>
        public AuthActionPolicyBuilder Protect(string actionName, params string[] roles)
        {
            if (!_policies.TryGetValue(actionName, out var settings))
            {
                settings = new AuthActionSettings();
                _policies[actionName] = settings;
            }
            settings.IsEnabled = true;
            settings.Roles = roles.Length > 0 ? roles : null;
            return this;
        }

        /// <summary>
        /// Vô hiệu hóa truy cập hoàn toàn vào action
        /// </summary>
        /// <param name="actionName">Tên action</param>
        public AuthActionPolicyBuilder Disable(string actionName)
        {
            if (!_policies.TryGetValue(actionName, out var settings))
            {
                settings = new AuthActionSettings();
                _policies[actionName] = settings;
            }
            settings.IsEnabled = false;
            return this;
        }

        /// <summary>
        /// Bỏ qua kiểm tra phân quyền (cho phép truy cập ẩn danh)
        /// </summary>
        /// <param name="actionName">Tên action</param>
        public AuthActionPolicyBuilder Bypass(string actionName)
        {
            _policies.Remove(actionName);
            return this;
        }
    }
}
