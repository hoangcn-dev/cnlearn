using HoangCN.BL.Interfaces;
using HoangCN.Common.Model.Entities;
using HoangCN.UserManagement.DTOs;
using HoangCN.UserManagement.Requests;
using System.Security.Claims;

namespace HoangCN.UserManagement.Interfaces
{
    public interface IUserService : IBaseBL<User>
    {
        Task SignUp(SignUpRequest request);
        Task SignIn(SignInRequest request);
        Task<LoginSessionInfoDto> GetLoginSessionInfo(Guid userId);
        Task<Guid> CheckAuth(ClaimsPrincipal claimsPrincipal);
        Task ForgotPassword(ForgotPasswordRequest request);
        Task ChangePassword(Guid userId, ChangePasswordRequest request);
    }
}
