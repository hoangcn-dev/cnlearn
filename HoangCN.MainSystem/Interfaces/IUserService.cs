using HoangCN.Core.BL.Interfaces;
using HoangCN.MainSystem.Entities;
using HoangCN.MainSystem.DTOs;
using HoangCN.MainSystem.Requests;
using System.Security.Claims;

namespace HoangCN.MainSystem.Interfaces
{
    public interface IUserService : IBaseBL<User>
    {
        Task SignUp(SignUpRequest request);
        Task SignIn(SignInRequest request);
        Task<LoginSessionInfoDto> GetLoginSessionInfo(Guid userId);
        Task<Guid> CheckAuth(ClaimsPrincipal claimsPrincipal);
        Task ForgotPassword(ForgotPasswordRequest request);
        Task ChangePassword(Guid userId, ChangePasswordRequest request);
        Task SignOut();
    }
}
