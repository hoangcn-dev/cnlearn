using HoangCN.Core.BL.Attributes.AuthAction;
using HoangCN.Core.BL.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.Common.Utils;
using HoangCN.LearnMS.DTOs;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;
using HoangCN.LearnMS.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HoangCN.LearnMS.Controllers
{
    /// <summary>
    /// API Quản lý ngân hàng đề trắc nghiệm và câu hỏi
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : CRUDController<Question>
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService) : base(questionService)
        {
            _questionService = questionService;
        }

        protected override void ConfigurePolicies(AuthActionPolicyBuilder builder)
        {
            builder.Disable(nameof(GetAll));
            builder.Disable(nameof(GetPaging));
            builder.Protect(nameof(Insert), nameof(RoleNames.Admin), nameof(RoleNames.User));
            builder.Protect(nameof(Update), nameof(RoleNames.Admin), nameof(RoleNames.User));
            builder.Protect(nameof(Delete), nameof(RoleNames.Admin), nameof(RoleNames.User));
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchQuestions([FromBody] GetRequest request, [FromQuery] bool isMine)
        {
            if (isMine)
            {
                var userId = ClaimUtil.GetUserId(User);
                if (userId == null)
                {
                    throw new UnauthorizedException("Vui lòng đăng nhập để tiếp tục"); 
                }

                request.Filters.Add(new Filter
                {
                    Property = nameof(Question.LearnMsUserId),
                    Operator = FilterOperator.Equal,
                    Value = userId.ToString(),
                    Type = FilterType.String
                });
            }
            var res = await _questionService.Get<QuestionDto>(request);
            return Ok(ApiResponseDto.Success(res));
        }

        public override async Task<IActionResult> GetById(Guid id)
        {
            var res = await _questionService.GetQuestionContent(ClaimUtil.GetUserId(User), id);
            return Ok(ApiResponseDto.Success(res));
        }

        [HttpPost("answers")]
        public async Task<IActionResult> GetQuestionAnswers(List<Guid> questionIds)
        {
            var res = await _questionService.GetAnswersContent(ClaimUtil.GetUserId(User), questionIds);
            return Ok(ApiResponseDto.Success(res));
        }

        [HttpPost("key")]
        public async Task<IActionResult> GetQuestionKey(List<Guid> questionIds)
        {
            var res = await _questionService.GetQuestionCorrectAnswer(ClaimUtil.GetUserId(User), questionIds);
            return Ok(ApiResponseDto.Success(res));
        }

        [HttpGet("saved")]
        [AuthAction]
        public async Task<IActionResult> GetSavedQuestions()
        {
            var userId = ClaimUtil.GetUserId(User);
            var res = await _questionService.GetSavedQuestions(userId!.Value);
            return Ok(ApiResponseDto.Success(res));
        }

        [HttpPost("saved")]
        [AuthAction]
        public async Task<IActionResult> GetSavedQuestions([FromBody] ToggleUserSavedRequest request)
        {
            var userId = ClaimUtil.GetUserId(User);
            request.UserId = userId!.Value;
            await _questionService.ToggleQuestion(request);
            return Ok(ApiResponseDto.Success(request.IsSaved? "Lưu thành công":"Bỏ lưu thành công"));
        }
    }
}
