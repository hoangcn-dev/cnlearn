using HoangCN.Common.Model.Entities;
using HoangCN.UserManagement.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ZLearn.Application.Common.Utils;

namespace HoangCN.UserManagement.Utils
{
    public class JwtConfig
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int AccessTokenTTL { get; set; } // min
        public int RefreshTokenTTL { get; set; } // min
    }

    public class JwtUtil
    {
        private readonly JwtConfig _jwtConfig;

        public JwtUtil(IOptions<JwtConfig> jwtConfig)
        {
            _jwtConfig = jwtConfig.Value;
        }

        public JwtTokenDTO IssueToken(User user, Role role, bool isLogin)
        {
            // Prepare claims
            List<Claim> claims = [
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, role.RoleName)
            ];

            // Prepare token key from secret key
            byte[] bytes = Encoding.UTF8.GetBytes(EnvUtil.GetValue(EnvVariableNames.JWT_SECRET_KEY));
            SymmetricSecurityKey tokenKey = new(bytes);

            // Expiration time
            DateTime expireAt = DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenTTL);

            // Gen new access token & refresh token
            JwtSecurityToken token = new
            (
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                signingCredentials: new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256),
                claims: claims,
                expires: expireAt
            );

            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            string refreshToken = GenerateRefreshToken();

            return new JwtTokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Type = "Bearer",
                ExpireAt = expireAt
            };
        }

        private string GenerateRefreshToken()
        {
            // Gen a random 32 chars string
            byte[] bytes = new byte[32];
            using var random = RandomNumberGenerator.Create();
            random.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public ClaimsPrincipal? ValidateAccessToken(string accessToken)
        {
            // Prepare params to validate expired accesstoken 
            byte[] bytes = Encoding.UTF8.GetBytes(EnvUtil.GetValue(EnvVariableNames.JWT_SECRET_KEY));
            SymmetricSecurityKey key = new(bytes);
            TokenValidationParameters param = new()
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtConfig.Issuer,

                ValidateAudience = true,
                ValidAudience = _jwtConfig.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,

                ValidateLifetime = false
            };

            // Validate
            try
            {
                ClaimsPrincipal claim = new JwtSecurityTokenHandler().ValidateToken(accessToken, param, out SecurityToken validatedToken);
                JwtSecurityToken? token = validatedToken as JwtSecurityToken;
                if (token is null || token.Header.Alg.ToLower() != SecurityAlgorithms.HmacSha256.ToLower())
                    return null;
                return claim;
            }
            catch
            {
                return null;
            }
        }
    }
}