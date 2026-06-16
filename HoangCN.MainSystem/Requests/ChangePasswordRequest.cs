using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.MainSystem.Requests
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại không được để trống.")]
        [DisplayName("Mật khẩu hiện tại")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải từ 6 ký tự trở lên.")]
        [DisplayName("Mật khẩu mới")]
        public string NewPassword { get; set; }
    }
}
