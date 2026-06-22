using HoangCN.Core.Common.Model.DTOs;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;
using HoangCN.LearnMS.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

using HoangCN.Core.BL.Base;

namespace HoangCN.LearnMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamSessionsController : CRUDController<ExamSession>
    {
        private readonly IExamSessionService _examSessionService;

        public ExamSessionsController(IExamSessionService examSessionService) : base(examSessionService)
        {
            _examSessionService = examSessionService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }
            return Guid.Empty;
        }

        [HttpPost("start")]
        [Authorize]
        public async Task<IActionResult> StartSession([FromBody] ExamSessionStartRequest request)
        {
            var candidateId = GetCurrentUserId();
            var sessionId = await _examSessionService.StartSessionAsync(candidateId, request);
            return Ok(ApiResponseDto.Success(sessionId));
        }

        [HttpPost("{sessionId}/heartbeat")]
        [Authorize]
        public async Task<IActionResult> Heartbeat(Guid sessionId, [FromBody] ExamSessionHeartbeatRequest request)
        {
            var candidateId = GetCurrentUserId();
            await _examSessionService.ProcessHeartbeatAsync(sessionId, candidateId, request);
            return Ok(ApiResponseDto.Success(true));
        }

        [HttpPost("{sessionId}/cheat-log")]
        [Authorize]
        public async Task<IActionResult> LogCheat(Guid sessionId, [FromBody] ExamCheatLogRequest request)
        {
            var candidateId = GetCurrentUserId();
            await _examSessionService.LogCheatAsync(sessionId, candidateId, request);
            return Ok(ApiResponseDto.Success(true));
        }

        [HttpPost("{sessionId}/submit")]
        [Authorize]
        public async Task<IActionResult> SubmitSession(Guid sessionId)
        {
            var candidateId = GetCurrentUserId();
            await _examSessionService.SubmitSessionAsync(sessionId, candidateId);
            return Ok(ApiResponseDto.Success(true));
        }
    }
}
