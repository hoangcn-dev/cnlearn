using API.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Module.Users.Models.Requests;
using Module.Users.Services;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class LogsController : ControllerBase
    {
        private readonly IUserService _userService;

        public LogsController(
            IUserService userService)
        {
            _userService = userService;
        }

        #region User logs

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUserLogs([FromQuery] SearchUserLogRequest request)
        {
            var users = await _userService.GetUserLogs(request);
            return Ok(ApiResponse.Success(users));
        }

        #endregion
    }
}