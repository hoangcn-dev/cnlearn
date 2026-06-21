using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.LearnMS.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HoangCN.LearnMS.Controllers
{
    [Route("api/attempts")]
    [ApiController]
    [Authorize]
    public class AttemptsController : ControllerBase
    {
        private readonly IExamAttemptService _attemptService;

        public AttemptsController(IExamAttemptService attemptService)
        {
            _attemptService = attemptService;
        }

        //[HttpPost("submit")]
        //public async Task<IActionResult> SubmitAttempt([FromBody] ExamAttemptSubmitDto submitDto)
        //{
        //    var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
        //    {
        //        throw new UnauthorizedException("Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
        //    }

        //    var result = await _attemptService.SubmitAttemptAsync(userId, submitDto);
            
        //    return Ok(ApiResponseDto.Success(result));
        //}

        //[HttpGet("history/exams")]
        //public async Task<IActionResult> GetExamHistory([FromQuery] int page = 1, [FromQuery] int size = 10)
        //{
        //    var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
        //    {
        //        throw new UnauthorizedException("Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
        //    }

        //    var result = await _attemptService.GetExamAttemptHistoryAsync(userId, page, size);
        //    return Ok(ApiResponseDto.Success(result));
        //}

        //[HttpGet("history/questions")]
        //public async Task<IActionResult> GetQuestionHistory([FromQuery] int page = 1, [FromQuery] int size = 10)
        //{
        //    var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
        //    {
        //        throw new UnauthorizedException("Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
        //    }

        //    var result = await _attemptService.GetQuestionAttemptHistoryAsync(userId, page, size);
        //    return Ok(ApiResponseDto.Success(result));
        //}
    }
}
