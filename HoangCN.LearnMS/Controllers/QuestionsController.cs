using HoangCN.Core.BL.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.Common.Utils;
using HoangCN.LearnMS.DTOs;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;
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
            builder.Disable(nameof(GetById));
            builder.Disable(nameof(GetPaging));
            builder.Protect(nameof(Insert), nameof(RoleNames.Admin));
            builder.Protect(nameof(Update), nameof(RoleNames.Admin));
            builder.Protect(nameof(Delete), nameof(RoleNames.Admin));
        }

        [HttpPost("bank/{id}")]
        public async Task<IActionResult> GetBankQuestionDetail(Guid id)
        {
            var res = await _questionService.GetBankQuestionWithAnswers(id);
            return Ok(ApiResponseDto.Success(res));
        }

        [HttpPost("bank/paging")]
        public async Task<IActionResult> GetBankQuestionPaging([FromBody] GetRequest request, [FromQuery] bool isMine)
        {
            if (isMine)
            {
                var userId = ClaimUtil.GetUserId(User);
                request.Filters.Add(new Filter
                {
                    Property = nameof(Question.LearnMsUserId),
                    Operator = FilterOperator.Equal,
                    Value = userId.ToString(),
                    Type = FilterType.String
                });
            }
            var res = await _baseBL.Get<BankQuestionDto>(request);
            return Ok(ApiResponseDto.Success(res));
        }
    }
}
