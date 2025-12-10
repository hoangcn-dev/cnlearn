using API.Models;
using Asp.Versioning;
using Core.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Module.Users.Models.Requests;
using Module.Users.Services;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public UsersController(
            IAuthService authService, 
            IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }



        #region For users

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetLoginInfo()
        {
            var info = _authService.GetLoginInfo(User);
            return Ok(ApiResponse.Success(info));
        }

        #endregion

        #region Auth

        [HttpGet("google-login")]
        public async Task<IActionResult> LoginWithGoolge([FromQuery] string returnUrl)
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(LoginWithGoolgeCallback), new {returnUrl}),
                Items = {
                    { "prompt", "select_account" }
                }
            };
            return Challenge(props, "Google");
        }

        [HttpGet("google-login-callback")]
        public async Task<IActionResult> LoginWithGoolgeCallback([FromQuery] string returnUrl)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("Google");
            if (authenticateResult is null)
            {
                return Redirect(StringUtil.AppendParamsToUrl(returnUrl, 
                    ("success", "false"),
                    ("message", "Đăng nhập thất bại")));
            }

            var token = await _authService.LoginWithGoogleAsync(authenticateResult);

            Response.Cookies.Append("aToken", token.AccessToken, new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                MaxAge = TimeSpan.FromMinutes(token.AccessTokenExpiryMin)
            });

            return Redirect(StringUtil.AppendParamsToUrl(returnUrl,
                    ("success", "true"),
                    ("message", "Đăng nhập thành công")));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.LoginAsync(request);

            Response.Cookies.Append("aToken", token.AccessToken, new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                MaxAge = TimeSpan.FromMinutes(token.AccessTokenExpiryMin)
            });

            return Ok(ApiResponse.Success("Đăng nhập thành công"));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.TryGetValue("aToken", out string? aToken)) {
                await _authService.LogoutAsync(aToken);
            }

            Response.Cookies.Delete("aToken");

            return Ok(ApiResponse.Success("Đăng xuất thành công"));
        }
        
        #endregion

        #region Manage users

        [HttpGet]
        [Authorize(Policy = StringUtil.PolicyNames.OnlyAdmin)]
        public async Task<IActionResult> GetAllUsers([FromQuery] SearchUserRequest request)
        {
            var users = await _userService.GetAllUsers(request);
            return Ok(ApiResponse.Success(users));
        }

        [HttpGet("{id}/detail")]
        [Authorize(Policy = StringUtil.PolicyNames.OnlyAdmin)]
        public async Task<IActionResult> GetUserDetail(string id)
        {
            var user = await _userService.GetUserDetail(Guid.Parse(id));
            return Ok(ApiResponse.Success(user));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = StringUtil.PolicyNames.OnlyAdmin)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            var updatedUserId = await _userService.UpdateUser(Guid.Parse(id), request);
            return Ok(ApiResponse.UpdateSuccess(updatedUserId));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = StringUtil.PolicyNames.OnlyAdmin)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var deletedUserId = await _userService.DeleteUser(Guid.Parse(id));
            return Ok(ApiResponse.DeleteSuccess(deletedUserId));
        }

        #endregion
    }
}
