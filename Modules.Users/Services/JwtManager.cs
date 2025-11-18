using Core.Exceptions;
using Core.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Modules.Users.Entities;
using Modules.Users.Models;
using Modules.Users.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Modules.Users.Services
{
    public class JwtManager
    {
        private JwtSettings _settings;

        public JwtManager(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        public SessionToken IssueToken(User user)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Name, $"{user.LastName} {user.FirstName}"),
                new (nameof(user.AvatarUrl), user.AvatarUrl),
                new (ClaimTypes.Email, user.Email),
                new (ClaimTypes.Role, user.Role.Name),
            };

            // Secret key
            byte[] bytes = Encoding.UTF8.GetBytes(EnvUtil.GetEnv(EnvUtil.Keys.HOANGCN_SECRET_KEY));
            var secretKey = new SymmetricSecurityKey(bytes);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256),
                expires: DateTimeOffset.UtcNow.AddMinutes(_settings.AccessTokenExpiryMin).UtcDateTime,
                claims: claims);

            var aToken = new JwtSecurityTokenHandler().WriteToken(token);
            var rToken = GenerateRefreshToken();

            return new SessionToken
            {
                AccessToken = aToken,
                AccessTokenExpiryMin = _settings.AccessTokenExpiryMin,
            };
        }

        public (bool, ClaimsPrincipal?) ValidateAToken(string aToken)
        {
            // Secret key
            byte[] bytes = Encoding.UTF8.GetBytes(EnvUtil.GetEnv(EnvUtil.Keys.HOANGCN_SECRET_KEY));
            var secretKey = new SymmetricSecurityKey(bytes);

            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _settings.Issuer,
                ValidAudience = _settings.Audience,
                IssuerSigningKey = secretKey,
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                ClaimsPrincipal principal = handler.ValidateToken(aToken, validationParams, out SecurityToken validatedToken);
                return (true, principal);
            }
            catch
            {
                return (false, null);
            }
        }

        public static TokenValidationParameters GetApiValidationTokenParams(IConfiguration config)
        {
            // Secret key
            byte[] bytes = Encoding.UTF8.GetBytes(EnvUtil.GetEnv(EnvUtil.Keys.HOANGCN_SECRET_KEY));
            var secretKey = new SymmetricSecurityKey(bytes);

            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["JwtSettings:Issuer"],
                ValidAudience = config["JwtSettings:Audience"],
                IssuerSigningKey = secretKey,
                ClockSkew = TimeSpan.Zero
            };
        }

        private string GenerateRefreshToken()
        {
            return StringUtil.GetRandomString(32);
        }
    }
}
