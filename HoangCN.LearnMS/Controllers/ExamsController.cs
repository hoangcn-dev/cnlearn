using HoangCN.Core.BL.Attributes.AuthAction;
using HoangCN.Core.BL.Base;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Utils;
using HoangCN.LearnMS.DTOs;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using HoangCN.Core.Common.Model.Requests;

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

        public ExamsController(
            IExamService examService) : base(examService)
        {
            _examService = examService;
        }

        protected override void ConfigurePolicies(AuthActionPolicyBuilder builder)
        {
            builder.Disable(nameof(GetAll));
            builder.Protect(nameof(Insert), nameof(RoleNames.Admin), nameof(RoleNames.User));
            builder.Protect(nameof(Update), nameof(RoleNames.Admin), nameof(RoleNames.User));
            builder.Protect(nameof(Delete), nameof(RoleNames.Admin), nameof(RoleNames.User));
        }

        [HttpPost("paging")]
        public override async Task<IActionResult> GetPaging([FromBody] GetRequest request)
        {
            var res = await _examService.Get<ExamDto>(request);
            return Ok(ApiResponseDto.Success(res));
        }

        /// <summary>
        /// Lấy chi tiết đề thi có kiểm tra quyền riêng tư
        /// </summary>
        [HttpGet("{id}")]
        public override async Task<IActionResult> GetById(Guid id)
        {
            var res = await _examService.GetExamByIdAsync(ClaimUtil.GetUserId(User), id);
            return Ok(ApiResponseDto.Success(res));
        }

        /// <summary>
        /// Lấy danh sách câu hỏi thuộc đề thi và xếp đúng thứ tự
        /// </summary>
        [HttpGet("{id}/questions")]
        public async Task<IActionResult> GetExamQuestions(Guid id)
        {
            var res = await _examService.GetExamQuestionsAsync(ClaimUtil.GetUserId(User), id);
            return Ok(ApiResponseDto.Success(res));
        }

        /// <summary>
        /// Lấy danh sách câu hỏi thuộc đề thi và xếp đúng thứ tự
        /// </summary>
        [HttpGet("{id}/answers")]
        public async Task<IActionResult> GetExamAnswer(Guid id)
        {
            var res = await _examService.GetAnswersContent(ClaimUtil.GetUserId(User), id);
            return Ok(ApiResponseDto.Success(res));
        }

        /// <summary>
        /// Lấy danh sách đáp án của đề thi
        /// </summary>
        [HttpGet("{id}/keys")]
        public async Task<IActionResult> GetExamAnswers(Guid id)
        {
            var res = await _examService.GetExamCorrectKeys(ClaimUtil.GetUserId(User), id);
            return Ok(ApiResponseDto.Success(res));
        }
    }
}
