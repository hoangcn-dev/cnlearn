using System;
using ZLearn.Application.Common.Utils;

namespace HoangCN.MainSystem.Utils
{
    /// <summary>
    /// Quản lý tập trung các Key của biến môi trường (Environment Variables) dành riêng cho phân hệ MainSystem
    /// </summary>
    public static class EnvKeyUtil
    {
        public const string JWT_SECRET_KEY = "JWT_SECRET_KEY";
        public const string HOANGCN_EMAIL_BOT_APP_PASSWORD = "HOANGCN_EMAIL_BOT_APP_PASSWORD";
        public const string HOANGCN_EMAIL_BOT_APP_EMAIL = "HOANGCN_EMAIL_BOT_APP_EMAIL";
        public const string DEFAULT_ADMIN_PASSWORD = "DEFAULT_ADMIN_PASSWORD";

        /// <summary>
        /// Lấy giá trị biến môi trường theo Key
        /// </summary>
        public static string GetValue(string key)
        {
            return EnvUtil.GetValue(key);
        }
    }
}
