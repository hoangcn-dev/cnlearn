using Core.Exceptions;
using Core.Interfaces;
using Core.Utilities;
using Microsoft.AspNetCore.Authentication;
using Modules.Users.Entities;
using Modules.Users.Models;
using System.Security.Claims;

namespace Modules.Users.Services
{
    public interface IAuthService
    {
        Task<SessionToken> LoginAsync(LoginRequest request);
        Task<SessionToken> LoginWithGoogleAsync(AuthenticateResult authenticateResult);
        LoginInfo GetLoginInfo(ClaimsPrincipal claims);
    }

    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtManager _jwtManager;

        public AuthService(IUnitOfWork unitOfWork, JwtManager jwtManager)
        {
            _unitOfWork = unitOfWork;
            _jwtManager = jwtManager;
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
                throw new NotFoundException(StringUtil.ErrorMessages.UserNotFound);

            if (!user.IsActived)
                throw new ForbiddenException(StringUtil.ErrorMessages.UserIsBanned);

            if (user.Password is null)
                throw new UnauthorizedException(StringUtil.ErrorMessages.PasswordNotSet);

            if (!PasswordHasher.VerifyPassword(request.Password, user.Password))
                throw new UnauthorizedException(StringUtil.ErrorMessages.PasswordIncorrect);

            var token = _jwtManager.IssueToken(user);
            user.LastLogin = DateTimeOffset.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return token;
        }

        public async Task<SessionToken> LoginWithGoogleAsync(AuthenticateResult authenticateResult)
        {
            var claims = authenticateResult.Principal?.Claims;

            if (claims is null)
                throw new UnauthorizedException(StringUtil.ErrorMessages.InvalidCredential);

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (email is null)
                throw new UnauthorizedException(StringUtil.ErrorMessages.InvalidCredential);

            var user = await _unitOfWork.Repository<User>()
                .GetFirstAsync(u => u.Email.Equals(email), includes: u => u.Role);

            // If user is null => create new one
            if (user is null)
            {
                var avatarUrl = claims.FirstOrDefault(c => c.Type == "image")?.Value;
                var firstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
                var lastName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;

                if (avatarUrl is null || firstName is null || lastName is null)
                    throw new UnauthorizedException(StringUtil.ErrorMessages.InvalidCredential);

                var userRole = await _unitOfWork.Repository<Role>()
                    .GetFirstAsync(r => r.Name.Equals(Roles.USER));

                if (userRole is null)
                    throw new ServerErrorException(StringUtil.ErrorMessages.UnknownError);

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
            }

            if (!user.IsActived)
                throw new ForbiddenException(StringUtil.ErrorMessages.UserIsBanned);

            var token = _jwtManager.IssueToken(user);
            user.LastLogin = DateTimeOffset.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return token;
        }
    }
}
