using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Exceptions;
using HoangCN.LearnMS.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using HoangCN.LearnMS.Interfaces;

namespace HoangCN.LearnMS.Controllers
{
    /// <summary>
    /// API Quản lý thông tin học viên cá nhân trong LearnMS
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController<LearnMsUser>
    {
        public UsersController(ILearnMsUserService userBL) : base(userBL)
        {
        }

        private Guid CheckAuth()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                throw new UnauthorizedException("Vui lòng đăng nhập để thực hiện chức năng này");
            }
            return userId;
        }

        /// <summary>
        /// Khởi tạo hoặc cập nhật nhanh thông tin user từ cổng đăng nhập tập trung
        /// </summary>
        [HttpPost("ensure")]
        public async Task<IActionResult> EnsureUserExists([FromBody] LearnMsUser userDto)
        {
            var authUserId = CheckAuth();
            
            // Ép buộc LearnMsUserId của dữ liệu gửi lên phải khớp với Token đang đăng nhập để bảo mật
            userDto.LearnMsUserId = authUserId;

            var getResult = await _baseBL.Get<LearnMsUser>(HoangCN.Core.Common.Model.Requests.GetRequest.GetByIdRequest(authUserId));
            if (getResult == null || getResult.Items.Count == 0)
            {
                await _baseBL.InsertAsync(new List<LearnMsUser> { userDto });
            }
            else
            {
                var existingUser = getResult.Items[0];
                existingUser.FullName = userDto.FullName;
                // Chặn việc sửa đổi Email và UserId ở mức logic
                await _baseBL.UpdateAsync(new List<LearnMsUser> { existingUser });
            }
            
            var latestResult = await _baseBL.Get<LearnMsUser>(HoangCN.Core.Common.Model.Requests.GetRequest.GetByIdRequest(authUserId));
            if (latestResult != null && latestResult.Items.Count > 0)
            {
                return Ok(ApiResponseDto.Success(latestResult.Items[0]));
            }
            
            return Ok(ApiResponseDto.Success(userDto));
        }

        /// <summary>
        /// Lấy hồ sơ cá nhân hiện tại của người dùng trong LearnMS
        /// </summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            var authUserId = CheckAuth();
            var res = await _baseBL.Get<LearnMsUser>(HoangCN.Core.Common.Model.Requests.GetRequest.GetByIdRequest(authUserId));
            if (res == null || res.Items.Count == 0)
            {
                return Ok(ApiResponseDto.Success(null));
            }
            return Ok(ApiResponseDto.Success(res.Items[0]));
        }

        /// <summary>
        /// Cập nhật thông tin cá nhân riêng biệt tại LearnMS
        /// </summary>
        [HttpPost("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] LearnMsUser userDto)
        {
            var authUserId = CheckAuth();
            userDto.LearnMsUserId = authUserId;
            
            var getResult = await _baseBL.Get<LearnMsUser>(HoangCN.Core.Common.Model.Requests.GetRequest.GetByIdRequest(authUserId));
            if (getResult == null || getResult.Items.Count == 0)
            {
                throw new NotFoundException("Tài khoản chưa được khởi tạo trên hệ thống");
            }

            var existingUser = getResult.Items[0];
            existingUser.FullName = userDto.FullName;

            await _baseBL.UpdateAsync(new List<LearnMsUser> { existingUser });
            return Ok(ApiResponseDto.Success());
        }
    }
}
