using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.MainSystem.DTOs;
using HoangCN.MainSystem.Entities;
using HoangCN.MainSystem.Enums;
using HoangCN.MainSystem.Interfaces;
using HoangCN.MainSystem.Models;
using HoangCN.MainSystem.Requests;
using HoangCN.MainSystem.Services;
using HoangCN.MainSystem.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HoangCN.MainSystem.Tests
{
    public class UserServiceTests
    {
        private readonly FakeReadDL _fakeReadDL;
        private readonly FakeWriteDL _fakeWriteDL;
        private readonly FakeRoleBL _fakeRoleBL;
        private readonly Mock<IHttpContextAccessor> _mockHttpAccessor;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IEmailTemplateService> _mockEmailTemplateService;
        private readonly Mock<IRedisService> _mockRedisService;
        private readonly JwtUtil _jwtUtil;
        
        private readonly EmailSettings _emailSettings;
        private readonly JwtConfig _jwtConfig;
        private readonly UserService _userService;

        private readonly Guid _adminRoleId = Guid.NewGuid();
        private readonly Guid _userRoleId = Guid.NewGuid();

        public UserServiceTests()
        {
            _fakeReadDL = new FakeReadDL();
            _fakeWriteDL = new FakeWriteDL();
            _fakeRoleBL = new FakeRoleBL();
            _mockHttpAccessor = new Mock<IHttpContextAccessor>();
            _mockEmailService = new Mock<IEmailService>();
            _mockEmailTemplateService = new Mock<IEmailTemplateService>();
            _mockRedisService = new Mock<IRedisService>();

            // Setup default roles
            var adminRole = new Role { RoleId = _adminRoleId, RoleName = nameof(RoleNames.Admin) };
            var userRole = new Role { RoleId = _userRoleId, RoleName = nameof(RoleNames.User) };
            _fakeRoleBL.Roles.Add(adminRole);
            _fakeRoleBL.Roles.Add(userRole);
            _fakeReadDL.QueryResults.Add(adminRole);
            _fakeReadDL.QueryResults.Add(userRole);

            _jwtConfig = new JwtConfig
            {
                Audience = "HoangCN.Webs",
                Issuer = "HoangCN",
                AccessTokenTTL = 60,
                RefreshTokenTTL = 120,
                CookieDomain = "localhost"
            };
            _jwtUtil = new JwtUtil(Options.Create(_jwtConfig));

            _emailSettings = new EmailSettings
            {
                SenderEmail = "bot@example.com",
                SenderName = "Bot",
                TemporaryPasswordExpireMin = 15
            };

            // By default, mock HttpContext to have a mock response and cookies
            var mockContext = new Mock<HttpContext>();
            var mockResponse = new Mock<HttpResponse>();
            var mockCookies = new Mock<IResponseCookies>();
            mockResponse.Setup(r => r.Cookies).Returns(mockCookies.Object);
            mockContext.Setup(c => c.Response).Returns(mockResponse.Object);

            // Mock default authenticated User claim to avoid ClaimUtil.GetUserName exception
            var claims = new List<Claim> { new Claim("UserName", "TestSystemUser") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            mockContext.Setup(c => c.User).Returns(principal);
            _mockHttpAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);

            _userService = new UserService(
                _fakeReadDL,
                _fakeWriteDL,
                _mockHttpAccessor.Object,
                _jwtUtil,
                _fakeRoleBL,
                _mockEmailService.Object,
                _mockEmailTemplateService.Object,
                Options.Create(_emailSettings),
                _mockRedisService.Object,
                Options.Create(_jwtConfig)
            );
        }

        #region 1. BeforeInsert & BeforeUpdate Tests

        [Fact]
        public async Task BeforeInsert_ShouldThrowBadRequestException_WhenActiveHttpContextAttemptsToCreateAdmin()
        {
            // Arrange
            var user = new User
            {
                UserName = "admin_user",
                Email = "admin@example.com",
                RoleId = _adminRoleId
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _userService.InsertAsync([user]));
        }

        [Fact]
        public async Task BeforeInsert_ShouldAllowAdminCreationAndHashPassword_WhenHttpContextIsNull_SystemSeeding()
        {
            // Arrange
            _mockHttpAccessor.Setup(a => a.HttpContext).Returns((HttpContext)null!); // HttpContext is null

            var user = new User
            {
                UserName = "admin_user",
                Email = "admin@example.com",
                Password = "plain_password",
                RoleId = _adminRoleId
            };

            // Act
            await _userService.InsertAsync([user]);

            // Assert
            Assert.NotEmpty(user.PasswordSalt);
            Assert.NotEqual("plain_password", user.Password);
            Assert.NotEmpty(user.UserCode);
            Assert.Contains(user, _fakeWriteDL.InsertedEntities.Cast<User>());
        }

        [Fact]
        public async Task BeforeUpdate_ShouldRetainOldPasswordAndSalt_WhenPasswordInPayloadIsEmpty()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var incomingUser = new User
            {
                UserId = userId,
                UserName = "test_user",
                UserCode = "U001",
                Email = "test@example.com",
                Password = "", // Empty in payload
                RoleId = _userRoleId,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };

            var dbUser = new User
            {
                UserId = userId,
                UserName = "test_user",
                UserCode = "U001",
                Email = "test@example.com",
                Password = "hashed_old_password",
                PasswordSalt = "old_salt",
                RoleId = incomingUser.RoleId,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };

            _fakeReadDL.QueryResults.Add(dbUser);

            // Act
            await _userService.UpdateAsync([incomingUser]);

            // Assert
            Assert.Equal("hashed_old_password", incomingUser.Password);
            Assert.Equal("old_salt", incomingUser.PasswordSalt);
            Assert.Contains(incomingUser, _fakeWriteDL.UpdatedEntities.Cast<User>());
        }

        [Fact]
        public async Task BeforeUpdate_ShouldHashNewPassword_WhenNewPasswordIsProvided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var incomingUser = new User
            {
                UserId = userId,
                UserName = "test_user",
                UserCode = "U001",
                Email = "test@example.com",
                Password = "brand_new_plain_password",
                RoleId = _userRoleId,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };

            var dbUser = new User
            {
                UserId = userId,
                UserName = "test_user",
                UserCode = "U001",
                Email = "test@example.com",
                Password = "hashed_old_password",
                PasswordSalt = "old_salt",
                RoleId = incomingUser.RoleId,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };

            _fakeReadDL.QueryResults.Add(dbUser);

            // Act
            await _userService.UpdateAsync([incomingUser]);

            // Assert
            Assert.NotEqual("brand_new_plain_password", incomingUser.Password);
            Assert.NotEqual("hashed_old_password", incomingUser.Password);
            Assert.NotEmpty(incomingUser.PasswordSalt);
            Assert.NotEqual("old_salt", incomingUser.PasswordSalt);
        }

        #endregion

        #region 2. SignUp Tests

        [Fact]
        public async Task SignUp_ShouldSucceed_WhenRequestIsValid()
        {
            // Arrange
            var request = new SignUpRequest
            {
                UserName = "newuser",
                Email = "new@example.com",
                DisplayName = "New User",
                Password = "password123"
            };

            // Act
            await _userService.SignUp(request);

            // Assert
            Assert.Contains(_fakeWriteDL.InsertedEntities.Cast<User>(), u => u.UserName == "newuser");
        }

        [Fact]
        public async Task SignUp_ShouldThrowBadRequestException_WhenUsernameOrEmailAlreadyExists()
        {
            // Arrange
            var request = new SignUpRequest
            {
                UserName = "existinguser",
                Email = "existing@example.com",
                DisplayName = "User",
                Password = "password"
            };

            var existingUser = new User { UserName = "existinguser", Email = "existing@example.com" };
            _fakeReadDL.QueryResults.Add(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _userService.SignUp(request));
        }

        #endregion

        #region 3. SignIn Tests

        [Fact]
        public async Task SignIn_ShouldSucceedAndSetCookie_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new SignInRequest
            {
                EmailOrUserName = "user1",
                Password = "plain_password"
            };

            var (hash, salt) = PasswordUtil.HashPassword("plain_password");
            var userAuth = new UserAuthDto
            {
                UserId = Guid.Parse("b7384285-f7ff-4320-a2d1-299319fdb39b"),
                UserName = "user1",
                UserCode = "U001",
                Email = "user1@example.com",
                Password = hash,
                PasswordSalt = salt,
                RoleName = "User",
                RoleId = _userRoleId,
                IsActive = true,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };

            _fakeReadDL.QueryResults.Add(userAuth);

            // Act
            await _userService.SignIn(request);

            // Assert
            _mockHttpAccessor.Verify(a => a.HttpContext!.Response.Cookies.Append("aToken", It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Once);
            Assert.Contains(_fakeWriteDL.UpdatedEntities.Cast<User>(), u => u.UserId == userAuth.UserId);
        }

        [Fact]
        public async Task SignIn_ShouldThrowUnauthorizedException_WhenUserIsLocked()
        {
            // Arrange
            var request = new SignInRequest
            {
                EmailOrUserName = "user1",
                Password = "plain_password"
            };

            var (hash, salt) = PasswordUtil.HashPassword("plain_password");
            var userAuth = new UserAuthDto
            {
                UserId = Guid.NewGuid(),
                UserName = "user1",
                UserCode = "U001",
                Email = "user1@example.com",
                Password = hash,
                PasswordSalt = salt,
                RoleName = "User",
                RoleId = _userRoleId,
                IsActive = false, // Locked
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };

            _fakeReadDL.QueryResults.Add(userAuth);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _userService.SignIn(request));
        }

        #endregion

        #region 4. ForgotPassword Tests

        [Fact]
        public async Task ForgotPassword_ShouldSendEmailAndSaveTempPasswordInRedis_WhenEmailExists()
        {
            // Arrange
            var request = new ForgotPasswordRequest { Email = "user@example.com" };
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = "user@example.com",
                DisplayName = "John Doe",
                IsActive = true
            };

            _fakeReadDL.QueryResults.Add(user);

            var emailTemplate = new EmailTemplate
            {
                TemplateCode = "forgot_password",
                Subject = "Forgot password for {{DisplayName}}",
                Content = "Temp: {{TemporaryPassword}} Expire: {{ExpireTime}}"
            };
            _mockEmailTemplateService.Setup(s => s.GetTemplateAsync("forgot_password")).ReturnsAsync(emailTemplate);

            // Act
            await _userService.ForgotPassword(request);

            // Assert
            _mockRedisService.Verify(r => r.SetAsync(It.Is<string>(k => k.Contains(user.UserId.ToString())), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Once);
            _mockEmailService.Verify(e => e.SendEmailAsync("user@example.com", It.Is<string>(s => s.Contains("John Doe")), It.IsAny<string>(), true), Times.Once);
        }

        #endregion

        #region 5. ChangePassword Tests

        [Fact]
        public async Task ChangePassword_ShouldSucceed_WhenCurrentPasswordIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new ChangePasswordRequest
            {
                CurrentPassword = "correct_password",
                NewPassword = "brand_new_password"
            };

            var (hash, salt) = PasswordUtil.HashPassword("correct_password");
            var user = new User
            {
                UserId = userId,
                UserName = "test_user",
                UserCode = "U001",
                Email = "test@example.com",
                Password = hash,
                PasswordSalt = salt,
                RoleId = _userRoleId,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };

            _fakeReadDL.QueryResults.Add(user);

            // Act
            await _userService.ChangePassword(userId, request);

            // Assert
            Assert.Contains(_fakeWriteDL.UpdatedEntities.Cast<User>(), u => u.UserId == userId);
        }

        [Fact]
        public async Task ChangePassword_ShouldThrowBadRequestException_WhenCurrentPasswordIsIncorrect()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new ChangePasswordRequest
            {
                CurrentPassword = "wrong_password",
                NewPassword = "brand_new_password"
            };

            var (hash, salt) = PasswordUtil.HashPassword("correct_password");
            var user = new User
            {
                UserId = userId,
                UserName = "test_user",
                UserCode = "U001",
                Email = "test@example.com",
                Password = hash,
                PasswordSalt = salt,
                RoleId = _userRoleId,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };

            _fakeReadDL.QueryResults.Add(user);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _userService.ChangePassword(userId, request));
        }

        #endregion
    }
}
