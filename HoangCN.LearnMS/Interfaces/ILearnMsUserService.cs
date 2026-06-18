using HoangCN.Core.BL.Interfaces;
using HoangCN.LearnMS.Entities;
using System.Security.Claims;

namespace HoangCN.LearnMS.Interfaces
{
    /// <summary>
    /// Giao diện nghiệp vụ cho thực thể LearnMsUser
    /// </summary>
    public interface ILearnMsUserService : IBaseBL<LearnMsUser>
    {
        /// <summary>
        /// Lấy thông tin hồ sơ người dùng, nếu chưa tồn lại thì đồng bộ lần đầu từ claims từ MainSystem
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        Task<LearnMsUser> GetProfile(ClaimsPrincipal claims);
    }
}
