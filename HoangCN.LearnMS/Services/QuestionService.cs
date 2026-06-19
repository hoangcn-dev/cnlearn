using Dapper;
using HoangCN.Core.BL.Base;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.BL.Utils;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.Common.Utils;
using HoangCN.Core.DL.Interfaces;
using HoangCN.LearnMS.DTOs;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Enums;
using HoangCN.LearnMS.Interfaces;
using HoangCN.LearnMS.Requests;

namespace HoangCN.LearnMS.Services
{
    /// <summary>
    /// Triển khai dịch vụ câu hỏi trắc nghiệm hỗ trợ import câu hỏi hàng loạt (bọc trong 1 Transaction duy nhất)
    /// </summary>
    public class QuestionService : BaseBL<Question>, IQuestionService
    {
        private readonly IBaseBL<QuestionCategory> _categoryService;
        private readonly IBaseBL<UserSavedMapping> _saveService;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(
            IBaseReadDL baseReadDL,
            IBaseWriteDL baseWriteDL,
            IBaseBL<QuestionCategory> categoryService,
            ILogger<QuestionService> logger,
            IHttpContextAccessor httpContextAccessor,
            IBaseBL<UserSavedMapping> saveService) : base(baseReadDL, baseWriteDL, httpContextAccessor)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _saveService = saveService;
        }

        protected override async Task BeforeInsert(List<Question> entities)
        {
            await base.BeforeInsert(entities);
            GenerateSlugForQuestions(entities);
        }

        protected override async Task BeforeUpdate(List<Question> entities)
        {
            await base.BeforeUpdate(entities);
            GenerateSlugForQuestions(entities);
        }

        private void GenerateSlugForQuestions(List<Question> entities)
        {
            foreach (var entity in entities)
            {
                if (entity == null) continue;

                // Tự sinh slug từ nội dung nếu rỗng
                if (string.IsNullOrWhiteSpace(entity.QuestionSlug))
                {
                    if (string.IsNullOrWhiteSpace(entity.StringContent))
                    {
                        throw new BadRequestException("Nội dung câu hỏi không được phép để trống.");
                    }
                    entity.QuestionSlug = SlugUtil.GenerateSlug(entity.StringContent);
                }
                else
                {
                    entity.QuestionSlug = SlugUtil.GenerateSlug(entity.QuestionSlug);
                }
            }
        }

        /// <summary>
        /// Lấy chi tiết một câu hỏi trong ngân hàng đề kèm theo toàn bộ đáp án của nó sử dụng Multiple Query
        /// </summary>
         public async Task<BankQuestionWithAnswersDto> GetBankQuestionWithAnswers(Guid questionId)
        {
            var param = new DynamicParameters();

            // Tạo câu sql lấy thông tin câu hỏi
            var questionQuery = BuildSQLUtil.BuildQueryStringGetDtoByCondition<Question, BankQuestionDto>(
                isGetOnlyOne: true,
                q => q.QuestionId == questionId,
                param);

            // Tạo câu sql lấy thông tin câu trả lời
            var answerQuery = BuildSQLUtil.BuildQueryStringGetDtoByCondition<QuestionAnswer, BankAnswerDto>(
                isGetOnlyOne: false,
                q => q.QuestionId == questionId,
                param);

            // Thực thi cả 2 câu cùng lúc trong cung 1 connnection
            var questionWithAnswers = await _baseReadDL.ExecuteQueryMultiple(
                $"{questionQuery}; {answerQuery};",
                async grid =>
                {
                    var question = (await grid.ReadAsync<BankQuestionWithAnswersDto>()).FirstOrDefault();
                    if (question == null)
                    {
                        return null;
                    }

                    var anwsers = (await grid.ReadAsync<BankAnswerDto>()).ToList();
                    question.Answers = anwsers;
                    return question;
                },
                param);

            if (questionWithAnswers == null)
            {
                throw new NotFoundException("Không tìm thấy câu hỏi");
            }

            return questionWithAnswers;
        }

        public async Task<List<SavedDto>> GetSavedQuestions(Guid userId)
        {
            var saveQuestions = await _saveService.GetByCondition<SavedDto>(
                s => s.UserId == userId && s.SaveType == SaveType.Question);
            return saveQuestions;
        }

        public async Task ToggleQuestion(ToggleUserSavedRequest request)
        {
            var mapping = await _saveService.GetSingleByCondition<UserSavedMapping>(
                s => s.UserId == request.UserId &&
                s.TargetId == request.TargetId);

            // Tránh trường hợp lưu lần 2
            if (request.IsSaved && mapping != null)
            {
                throw new BadRequestException("Câu hỏi đã được lưu");
            }

            // Chặn trường hợp bỏ lưu khi chưa lưu lần nào
            if (!request.IsSaved && mapping == null)
            {
                throw new BadRequestException("Câu hỏi chưa được lưu");
            }

            // Đảm bảo Id câu hỏi có tồn tại
            var qCnt = await CountByCondition(q => q.QuestionId == request.TargetId); 
            if (qCnt == 0)
            {
                throw new BadRequestException("Câu hỏi không tồn tại");
            }

            if (request.IsSaved)
            {
                await _saveService.InsertAsync([new () {
                    TargetId = request.TargetId,
                    SaveType = SaveType.Question,
                    UserId = request.UserId
                }]);
            }
            else
            {
                await _saveService.DeleteAsync(new DeleteRequest()
                {
                    Ids = [ mapping!.UserSavedMappingId ]
                });
            }
            
        }
    }
}


