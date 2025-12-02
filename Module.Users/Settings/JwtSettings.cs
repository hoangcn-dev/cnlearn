namespace Module.Users.Settings
{
    public class JwtSettings
    {
        public int AccessTokenExpiryMin { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
