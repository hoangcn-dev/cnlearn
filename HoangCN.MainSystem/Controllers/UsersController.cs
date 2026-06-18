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
    [Authorize]
    public class UsersController : BaseController<User>
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService) : base(userService)
        {
            _userService = userService;
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            await _userService.SignUp(request);
            return Ok(ApiResponseDto.Success("Đăng ký tài khoản thành công."));
        }

        [HttpPost("signin")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            await _userService.SignIn(request);
            return Ok(ApiResponseDto.Success("Đăng nhập thành công."));
        }


        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            await _userService.ForgotPassword(request);
            return Ok(ApiResponseDto.Success("Mật khẩu tạm thời đã được gửi tới email của bạn. Vui lòng kiểm tra hộp thư."));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = await _userService.CheckAuth(User);
            var info = await _userService.GetLoginSessionInfo(userId);
            return Ok(ApiResponseDto.Success(info));
        }

        [HttpPost("logout")]
        [AllowAnonymous]
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

        #region Hạn chế quyền Admin đối với các tác vụ CRUD người dùng

        [HttpGet]
        [Authorize(Roles = nameof(RoleNames.Admin))]
        public override async Task<IActionResult> GetAll()
        {
            return await base.GetAll();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = nameof(RoleNames.Admin))]
        public override async Task<IActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        [HttpPost("paging")]
        [Authorize(Roles = nameof(RoleNames.Admin))]
        public override async Task<IActionResult> GetPaging([FromBody] GetRequest request)
        {
            return await base.GetPaging(request);
        }

        [HttpPost]
        [Authorize(Roles = nameof(RoleNames.Admin))]
        public override async Task<IActionResult> Insert([FromBody] List<User> entities)
        {
            return await base.Insert(entities);
        }

        [HttpPut]
        [Authorize(Roles = nameof(RoleNames.Admin))]
        public override async Task<IActionResult> Update([FromBody] List<User> entities)
        {
            return await base.Update(entities);
        }

        [HttpPost("delete")]
        [Authorize(Roles = nameof(RoleNames.Admin))]
        public override async Task<IActionResult> Delete([FromBody] DeleteRequest request)
        {
            return await base.Delete(request);
        }

        #endregion
    }
}

