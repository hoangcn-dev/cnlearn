namespace HoangCN.UserManagement.DTOs
{
    public class JwtTokenDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Type { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
