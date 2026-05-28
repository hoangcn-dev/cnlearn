using System.ComponentModel.DataAnnotations;

namespace HoangCN.UserManagement.Requests
{
    public class SignInRequest
    {
        [Required]
        public string EmailOrUserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
