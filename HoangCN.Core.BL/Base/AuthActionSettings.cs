namespace HoangCN.Core.BL.Base
{
    /// <summary>
    /// Định nghĩa các thiết lập cho các action có trong BaseController
    /// </summary>
    public class AuthActionSettings
    {
        /// <summary>
        /// Dùng API này hay không, mặc định là true. Nếu false thì action sẽ bị vô hiệu hóa
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Danh sách các role được phép truy cập vào action này. Nếu null hoặc rỗng thì tất cả các role đều được phép truy cập
        /// </summary>
        public string[]? Roles { get; set; } = null;
    }
}
