using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using HoangCN.Core.BL.Base;

namespace HoangCN.LearnMS.Controllers
{
    /// <summary>
    /// API Quản lý kỳ thi / bài kiểm tra
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class QuizzesController : CRUDController<Quiz>
    {
        private readonly IQuizService _quizService;

        public QuizzesController(IQuizService quizService) : base(quizService)
        {
            _quizService = quizService ?? throw new ArgumentNullException(nameof(quizService));
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid parsedId))
            {
                return parsedId;
            }
            return null;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var request = GetRequest.GetAllRequest();
            var res = await _quizService.GetQuizzesPagingAsync(request, GetCurrentUserId());
            return Ok(ApiResponseDto.Success(res));
        }

        [HttpPost("paging")]
        public override async Task<IActionResult> GetPaging([FromBody] GetRequest request)
        {
            var res = await _quizService.GetQuizzesPagingAsync(request, GetCurrentUserId());
            return Ok(ApiResponseDto.Success(res));
        }

        /// <summary>
        /// Ghi đè phương thức Insert để gán UserId hiện tại
        /// </summary>
        [HttpPost]
        public override async Task<IActionResult> Insert([FromBody] List<Quiz> entities)
        {
            Guid userId = Guid.Empty;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid parsedId))
            {
                userId = parsedId;
            }

            foreach (var entity in entities)
            {
                if (entity.UserId == Guid.Empty)
                {
                    entity.UserId = userId;
                }
            }

            return await base.Insert(entities);
        }
    }
}
