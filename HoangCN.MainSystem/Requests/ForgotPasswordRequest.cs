using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.MainSystem.Requests
{
    /// <summary>
    /// Yêu cầu khôi phục mật khẩu quên
    /// </summary>
    public class ForgotPasswordRequest
    {
        /// <summary>
        /// Email đã đăng ký trong hệ thống
        /// </summary>
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng.")]
        [DisplayName("Email")]
        public string Email { get; set; } = string.Empty;
    }
}
