namespace HoangCN.MainSystem.Models
{
    /// <summary>
    /// Cấu hình SMTP Email
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// Địa chỉ máy chủ SMTP
        /// </summary>
        public string SmtpServer { get; set; } = string.Empty;

        /// <summary>
        /// Cổng máy chủ SMTP (ví dụ: 587, 465)
        /// </summary>
        public int SmtpPort { get; set; }

        /// <summary>
        /// Tên người gửi hiển thị
        /// </summary>
        public string SenderName { get; set; } = string.Empty;

        /// <summary>
        /// Email của người gửi
        /// </summary>
        public string SenderEmail { get; set; } = string.Empty;

        /// <summary>
        /// Tên đăng nhập SMTP
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu đăng nhập SMTP
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Có sử dụng SSL/TLS hay không
        /// </summary>
        public bool EnableSsl { get; set; } = true;

        /// <summary>
        /// Thời gian sống của mật khẩu tạm thời (đơn vị: phút)
        /// </summary>
        public int TemporaryPasswordExpireMin { get; set; } = 15;
    }
}
