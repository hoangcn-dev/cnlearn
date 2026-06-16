using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.MainSystem.Requests
{
    public class SignInRequest
    {
        [Required(ErrorMessage = "Tài khoản hoặc email không được để trống.")]
        [DisplayName("Tài khoản hoặc email")]
        public string EmailOrUserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [DisplayName("Mật khẩu")]
        public string Password { get; set; }
    }
}
