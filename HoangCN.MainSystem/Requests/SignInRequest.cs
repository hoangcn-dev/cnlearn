using System.ComponentModel.DataAnnotations;

namespace HoangCN.MainSystem.Requests
{
    public class SignInRequest
    {
        [Required]
        public string EmailOrUserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
