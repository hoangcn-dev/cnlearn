using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.Common.Enums;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;
using HoangCN.LearnMS.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using HoangCN.Core.BL.Base;

namespace HoangCN.LearnMS.Controllers
{
    /// <summary>
    /// API Quản lý đề thi
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : CRUDController<Exam>
    {
        private readonly IExamService _examService;
        private readonly IBaseBL<ExamQuestion> _examQuestionBL;
        private readonly IQuestionService _questionService;

        public ExamsController(
            IExamService examService,
            IBaseBL<ExamQuestion> examQuestionBL,
            IQuestionService questionService) : base(examService)
        {
            _examService = examService ?? throw new ArgumentNullException(nameof(examService));
            _examQuestionBL = examQuestionBL ?? throw new ArgumentNullException(nameof(examQuestionBL));
            _questionService = questionService ?? throw new ArgumentNullException(nameof(questionService));
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
            var res = await _examService.GetExamsPagingAsync(request, GetCurrentUserId());
            return Ok(ApiResponseDto.Success(res));
        }

        [HttpPost("paging")]
        public override async Task<IActionResult> GetPaging([FromBody] GetRequest request)
        {
            var res = await _examService.GetExamsPagingAsync(request, GetCurrentUserId());
            return Ok(ApiResponseDto.Success(res));
        }

        /// <summary>
        /// Lấy số lượng câu hỏi của tất cả đề thi
        /// </summary>
        [HttpGet("question-counts")]
        public async Task<IActionResult> GetQuestionCounts()
        {
            var counts = await _examService.GetQuestionCountsAsync();
            return Ok(ApiResponseDto.Success(counts));
        }

        /// <summary>
        /// Lưu chi tiết đề thi và danh sách câu hỏi đi kèm
        /// </summary>
        [HttpPost("save-details")]
        public async Task<IActionResult> SaveDetails([FromBody] ExamSaveDto dto)
        {
            Guid userId = Guid.Empty;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid parsedId))
            {
                userId = parsedId;
            }

            var examId = await _examService.SaveExamDetailsAsync(dto, userId);
            return Ok(ApiResponseDto.Success(new { ExamId = examId }));
        }

        ///// <summary>
        ///// Lấy danh sách câu hỏi thuộc đề thi
        ///// </summary>
        //[HttpGet("{id}/questions")]
        //public async Task<IActionResult> GetExamQuestions(Guid id)
        //{
        //    Guid userId = Guid.Empty;
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid parsedId))
        //    {
        //        userId = parsedId;
        //    }

        //    var sortedQuestions = await _questionService.GetQuestionsByExamIdAsync(id, userId);

        //    return Ok(ApiResponseDto.Success(sortedQuestions));
        //}
    }
}
