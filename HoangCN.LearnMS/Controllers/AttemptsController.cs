using HoangCN.Core.Common.Exceptions;
using HoangCN.LearnMS.DTOs.Attempts;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

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

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitAttempt([FromBody] ExamAttemptSubmitDto submitDto)
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
                {
                    throw new UnauthorizedException("Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
                }

                var result = await _attemptService.SubmitAttemptAsync(userId, submitDto);
                
                return Ok(new
                {
                    IsSuccess = true,
                    Data = result
                });
            }
            catch (UnauthorizedException ex)
            {
                return StatusCode(401, new
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    IsSuccess = false,
                    ErrorMessage = "Lỗi khi nộp bài: " + ex.Message
                });
            }
        }

        [HttpGet("history/exams")]
        public async Task<IActionResult> GetExamHistory([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
                {
                    throw new UnauthorizedException("Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
                }

                var result = await _attemptService.GetExamAttemptHistoryAsync(userId, page, size);
                return Ok(new
                {
                    IsSuccess = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    IsSuccess = false,
                    ErrorMessage = "Lỗi khi lấy lịch sử bài thi: " + ex.Message
                });
            }
        }

        [HttpGet("history/questions")]
        public async Task<IActionResult> GetQuestionHistory([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
                {
                    throw new UnauthorizedException("Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
                }

                var result = await _attemptService.GetQuestionAttemptHistoryAsync(userId, page, size);
                return Ok(new
                {
                    IsSuccess = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    IsSuccess = false,
                    ErrorMessage = "Lỗi khi lấy lịch sử câu hỏi: " + ex.Message
                });
            }
        }
    }
}
