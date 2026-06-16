using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.MainSystem.Requests
{
    public class SignUpRequest
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng.")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên.")]
        [DisplayName("Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Tên hiển thị không được để trống.")]
        [DisplayName("Tên hiển thị")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "Tên tài khoản không được để trống.")]
        [DisplayName("Tên tài khoản")]
        public string UserName { get; set; }
    }
}
