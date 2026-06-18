using HoangCN.MainSystem.Entities;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.MainSystem.Interfaces;
using HoangCN.MainSystem.Requests;
using HoangCN.MainSystem.Enums;
using HoangCN.MainSystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HoangCN.Core.BL.Base;

namespace HoangCN.MainSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : CRUDController<User>
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService) : base(userService)
        {
            _userService = userService;
        }

        protected override void ConfigurePolicies(AuthActionPolicyBuilder builder)
        {
            builder.Protect(nameof(GetAll), nameof(RoleNames.Admin));
            builder.Protect(nameof(GetById), nameof(RoleNames.Admin));
            builder.Protect(nameof(GetPaging), nameof(RoleNames.Admin));
            builder.Protect(nameof(Insert), nameof(RoleNames.Admin));
            builder.Protect(nameof(Update), nameof(RoleNames.Admin));
            builder.Protect(nameof(Delete), nameof(RoleNames.Admin));
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            await _userService.SignUp(request);
            return Ok(ApiResponseDto.Success("Đăng ký tài khoản thành công."));
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            await _userService.SignIn(request);
            return Ok(ApiResponseDto.Success("Đăng nhập thành công."));
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            await _userService.ForgotPassword(request);
            return Ok(ApiResponseDto.Success("Mật khẩu tạm thời đã được gửi tới email của bạn. Vui lòng kiểm tra hộp thư."));
        }

        [HttpGet("me")]
        [AuthAction]
        public async Task<IActionResult> GetMe()
        {
            var userId = await _userService.CheckAuth(User);
            var info = await _userService.GetLoginSessionInfo(userId);
            return Ok(ApiResponseDto.Success(info));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> SignOut()
        {
            await _userService.SignOut();
            return Ok(ApiResponseDto.Success("Đăng xuất thành công."));
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = await _userService.CheckAuth(User);
            await _userService.ChangePassword(userId, request);
            return Ok(ApiResponseDto.Success("Đổi mật khẩu thành công."));
        }
    }
}

