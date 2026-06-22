using System;
using System.Security.Claims;
using HoangCN.MainSystem.DTOs;
using HoangCN.MainSystem.Entities;
using HoangCN.MainSystem.Utils;
using Microsoft.Extensions.Options;
using Xunit;

namespace HoangCN.MainSystem.Tests
{
    public class JwtUtilTests
    {
        private readonly JwtUtil _jwtUtil;
        private readonly JwtConfig _jwtConfig;

        public JwtUtilTests()
        {
            _jwtConfig = new JwtConfig
            {
                Audience = "HoangCN.Webs",
                Issuer = "HoangCN",
                AccessTokenTTL = 60,
                RefreshTokenTTL = 120,
                CookieDomain = "localhost"
            };

            var options = Options.Create(_jwtConfig);
            _jwtUtil = new JwtUtil(options);
        }

        [Fact]
        public void IssueToken_ShouldCreateTokenWithCorrectClaims()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com"
            };

            var role = new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = "Admin"
            };

            // Act
            var tokenDto = _jwtUtil.IssueToken(user, role, true);

            // Assert
            Assert.NotNull(tokenDto);
            Assert.NotEmpty(tokenDto.AccessToken);
            Assert.NotEmpty(tokenDto.RefreshToken);
            Assert.Equal("Bearer", tokenDto.Type);
            Assert.True(tokenDto.ExpireAt > DateTime.UtcNow);

            // Validate token contents
            var claimsPrincipal = _jwtUtil.ValidateAccessToken(tokenDto.AccessToken);
            Assert.NotNull(claimsPrincipal);

            var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = claimsPrincipal.FindFirst(ClaimTypes.Role);
            var userNameClaim = claimsPrincipal.FindFirst("UserName");

            Assert.NotNull(userIdClaim);
            Assert.Equal(user.UserId.ToString(), userIdClaim.Value);
            
            Assert.NotNull(roleClaim);
            Assert.Equal(role.RoleName, roleClaim.Value);

            Assert.NotNull(userNameClaim);
            Assert.Equal(user.UserName, userNameClaim.Value);
        }

        [Fact]
        public void ValidateAccessToken_ShouldReturnNull_WhenTokenIsInvalid()
        {
            // Act
            var claimsPrincipal = _jwtUtil.ValidateAccessToken("invalid_token_string");

            // Assert
            Assert.Null(claimsPrincipal);
        }
    }
}
