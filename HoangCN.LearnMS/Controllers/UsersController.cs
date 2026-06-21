using HoangCN.Core.BL.Attributes.AuthAction;
using HoangCN.Core.BL.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Utils;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HoangCN.LearnMS.Controllers
{
    /// <summary>
    /// API Quản lý thông tin học viên cá nhân trong LearnMS
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : CRUDController<LearnMsUser>
    {
        private readonly ILearnMsUserService _userBL;

        public UsersController(ILearnMsUserService userBL) : base(userBL)
        {
            _userBL = userBL;
        }

        protected override void ConfigurePolicies(AuthActionPolicyBuilder builder)
        {
            builder.Protect(nameof(GetAll), nameof(RoleNames.Admin))
                   .Protect(nameof(GetById), nameof(RoleNames.Admin))
                   .Protect(nameof(GetPaging), nameof(RoleNames.Admin))
                   .Protect(nameof(Insert), nameof(RoleNames.Admin))
                   .Protect(nameof(Update))
                   .Protect(nameof(Delete), nameof(RoleNames.Admin));
        }

        /// <summary>
        /// Lấy hồ sơ cá nhân hiện tại của người dùng trong LearnMS
        /// </summary>
        [HttpGet("profile")]
        [AuthAction]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            var res = await _userBL.GetProfile(User);
            return Ok(ApiResponseDto.Success(res));
        }

        /// <summary>
        /// Cập nhật thông tin cá nhân riêng biệt tại LearnMS
        /// </summary>
        [HttpPost("profile")]
        [AuthAction]
        public async Task<IActionResult> UpdateProfile([FromBody] LearnMsUser userDto)
        {
            var userId = ClaimUtil.GetUserId(User)
                ?? throw new UnauthorizedException("Vui lòng đăng nhập để tiếp tục");

            if (userDto.LearnMsUserId != Guid.Empty && userDto.LearnMsUserId != Guid.Empty && userDto.LearnMsUserId != userId)
            {
                throw new ForbiddenException("Bạn không có quyền cập nhật thông tin của người khác");
            }

            userDto.LearnMsUserId = userId;
            await Update([userDto]);
            return Ok(ApiResponseDto.Success("Cập nhật thông tin cá nhân thành công"));
        }
    }
}
