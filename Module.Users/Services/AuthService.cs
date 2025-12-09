using Core.Base;
using Core.Exceptions;
using Core.Interfaces;
using Core.Utilities;
using Microsoft.AspNetCore.Authentication;
using Module.Users.Entities;
using Module.Users.Models;
using Module.Users.Models.Requests;
using System.Security.Claims;

namespace Module.Users.Services
{
    public interface IAuthService
    {
        Task<SessionToken> LoginAsync(LoginRequest request);
        Task<SessionToken> LoginWithGoogleAsync(AuthenticateResult authenticateResult);
        Task LogoutAsync(string accessToken);
        LoginInfo GetLoginInfo(ClaimsPrincipal claims);
    }

    public class AuthService : BaseService, IAuthService
    {
        private readonly JwtService _jwtService;

        public AuthService(
            IUnitOfWork unitOfWork, JwtService jwtService) : base(unitOfWork)
        {
            _jwtService = jwtService;
        }

        public LoginInfo GetLoginInfo(ClaimsPrincipal claims)
        {
            return new LoginInfo
            {
                FullName = claims.FindFirstValue(ClaimTypes.Name),
                AvatarUrl = claims.FindFirstValue(nameof(User.AvatarUrl)),
                Email = claims.FindFirstValue(ClaimTypes.Email),
                Role = claims.FindFirstValue(ClaimTypes.Role),
            };
        }

        public async Task<SessionToken> LoginAsync(LoginRequest request)
        {
            var user = await _unitOfWork.Repository<User>()
                .GetFirstAsync(u => u.Email.Equals(request.Email), includes: u => u.Role);

            if (user is null)
            {
                throw new NotFoundException(StringUtil.ApiMessages.UserNotFound);
            }

            if (!user.IsActived)
            {
                throw new ForbiddenException(StringUtil.ApiMessages.UserIsBanned);
            }

            if (user.Password is null)
            {
                throw new UnauthorizedException(StringUtil.ApiMessages.PasswordNotSet);
            }

            if (!PasswordHasher.VerifyPassword(request.Password, user.Password))
            {
                throw new UnauthorizedException(StringUtil.ApiMessages.PasswordIncorrect);
            }

            var token = _jwtService.IssueToken(user);
            user.LastLogin = DateTimeOffset.UtcNow;
            user.AddLog(StringUtil.LogMessages.UserLoggedIn);

            await _unitOfWork.Repository<User>().UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return token;
        }

        public async Task<SessionToken> LoginWithGoogleAsync(AuthenticateResult authenticateResult)
        {
            var claims = authenticateResult.Principal?.Claims;

            if (claims is null)
            {
                throw new UnauthorizedException(StringUtil.ApiMessages.InvalidCredential);
            }

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (email is null)
            {
                throw new UnauthorizedException(StringUtil.ApiMessages.InvalidCredential);
            }

            var user = await _unitOfWork.Repository<User>()
                .GetFirstAsync(u => u.Email.Equals(email), includes: u => u.Role);

            // If user is null => create new one
            if (user is null)
            {
                var avatarUrl = claims.FirstOrDefault(c => c.Type == "image")?.Value;
                var firstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
                var lastName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;

                if (avatarUrl is null || firstName is null || lastName is null)
                {
                    throw new UnauthorizedException(StringUtil.ApiMessages.InvalidCredential);
                }

                var userRole = await _unitOfWork.Repository<Role>()
                    .GetFirstAsync(r => r.Name.Equals(Roles.USER));

                if (userRole is null)
                {
                    throw new ServerErrorException(StringUtil.ApiMessages.UnknownError);
                }

                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    AvatarUrl = avatarUrl,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = null,
                    Password = null,
                    Role = userRole,
                    IsActived = true
                };

                user.AddLog(StringUtil.LogMessages.UserCreatedFromGoogleAccount);
            }

            if (!user.IsActived)
            {
                throw new ForbiddenException(StringUtil.ApiMessages.UserIsBanned);
            }

            var token = _jwtService.IssueToken(user);
            user.LastLogin = DateTimeOffset.UtcNow;
            user.AddLog(StringUtil.LogMessages.UserLoggedIn);

            await _unitOfWork.Repository<User>().UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return token;
        }

        public async Task LogoutAsync(string accessToken)
        {
            var (valid, claims) = _jwtService.ValidateAToken(accessToken);
            if (valid && claims!.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _unitOfWork.Repository<User>().GetFirstAsync(
                    predicate: u => u.Id.ToString() == userId
                );

                if (user is not null)
                {
                    user.IsOnline = false;
                    user.AddLog(StringUtil.LogMessages.UserLoggedOut);

                    await _unitOfWork.Repository<User>().UpdateAsync(user);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }
    }
}
