using System;
using System.Collections.Generic;
using System.Reflection;

namespace HoangCN.Core.BL.Base
{
    /// <summary>
    /// Bộ dựng cấu hình phân quyền (Fluent API) cho các Action trong BaseController
    /// </summary>
    public class AuthActionPolicyBuilder
    {
        private readonly Dictionary<string, AuthActionSettings> _policies;
        private readonly Type _controllerType;

        public AuthActionPolicyBuilder(Dictionary<string, AuthActionSettings> policies, Type controllerType)
        {
            _policies = policies;
            _controllerType = controllerType;

            // Kiểm tra xem Controller hiện tại có kế thừa từ BaseController hay không
            if (!typeof(BaseController).IsAssignableFrom(controllerType))
            {
                throw new ArgumentException($"Controller '{controllerType.Name}' không kế thừa từ BaseController.");
            }
        }

        private void ValidateActionName(string actionName)
        {
            if (string.IsNullOrEmpty(actionName))
            {
                throw new ArgumentException("Tên action không được để trống.");
            }

            try
            {
                var baseType = _controllerType.BaseType;
                if (baseType == null)
                {
                    throw new ArgumentException($"Không tìm thấy Controller cha cho Controller '{_controllerType.Name}'.");
                }

                // GetMethod sẽ quét trên lớp cha và toàn bộ cây kế thừa của lớp cha, hoàn toàn loại trừ lớp con hiện tại.
                var method = baseType.GetMethod(actionName, BindingFlags.Public | BindingFlags.Instance);
                if (method == null)
                {
                    throw new ArgumentException($"Phương thức '{actionName}' không tồn tại trong Controller cha.");
                }
            }
            catch (AmbiguousMatchException)
            {
                // Nếu ném ra AmbiguousMatchException có nghĩa là có nhiều phương thức trùng tên -> phương thức đó chắc chắn tồn tại ở lớp cha
            }
        }

        /// <summary>
        /// Bảo vệ action: Yêu cầu đăng nhập và có vai trò cụ thể (nếu có truyền vào)
        /// </summary>
        /// <param name="actionName">Tên action (Ví dụ: nameof(Insert))</param>
        /// <param name="roles">Danh sách các vai trò được phép truy cập</param>
        public AuthActionPolicyBuilder Protect(string actionName, params string[] roles)
        {
            ValidateActionName(actionName);

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
            ValidateActionName(actionName);

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
            ValidateActionName(actionName);

            _policies.Remove(actionName);
            return this;
        }
    }
}
