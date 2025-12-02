namespace Module.Users.Models
{
    public class SessionToken
    {
        public string AccessToken { get; set; }
        public int AccessTokenExpiryMin { get; set; }
    }
}
