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

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetLoginInfo()
        {
            var info = _authService.GetLoginInfo(User);
            return Ok(ApiResponse.Success(info));
        }

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
            Response.Cookies.Delete("aToken");

            return Ok(ApiResponse.Success("Đăng xuất thành công"));
        }
    }
}
