using Dapper;
using HoangCN.Core.BL.Base;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.Common.Utils;
using HoangCN.Core.DL.Interfaces;
using HoangCN.Core.DL.Utils;
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
        private readonly IBaseBL<QuestionAnswer> _answerService;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(
            IBaseReadDL baseReadDL,
            IBaseWriteDL baseWriteDL,
            IBaseBL<QuestionCategory> categoryService,
            ILogger<QuestionService> logger,
            IHttpContextAccessor httpContextAccessor,
            IBaseBL<UserSavedMapping> saveService,
            IBaseBL<QuestionAnswer> answerService) : base(baseReadDL, baseWriteDL, httpContextAccessor)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _saveService = saveService;
            _answerService = answerService;
        }

        protected override async Task BeforeInsert(List<Question> entities)
        {
            await base.BeforeInsert(entities);
            GenerateSlugForQuestions(entities);
            await ValidateLeafCategories(entities);
            AssignAuthor(entities);
            AssignQuestionAnswer(entities);
        }

        protected override async Task BeforeUpdate(List<Question> entities)
        {
            await base.BeforeUpdate(entities);
            GenerateSlugForQuestions(entities);
            await ValidateLeafCategories(entities);
            AssignAuthor(entities);
            AssignQuestionAnswer(entities);
        }

        private void AssignQuestionAnswer(List<Question> questions)
        {
            foreach (var question in questions)
            {
                foreach(var answer in question.Answers)
                {
                    answer.QuestionId = question.QuestionId;
                }
            }
        }

        private void AssignAuthor(List<Question> questions)
        {
            var userId = ClaimUtil.GetUserId(_httpContextAccessor.HttpContext?.User);
            if (userId == null)
            {
                throw new UnauthorizedException("Vui lòng đăng nhập để tiếp tục");
            }
            foreach (var question in questions)
            {
                question.LearnMsUserId = userId!.Value;
            }
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

        private async Task ValidateLeafCategories(List<Question> entities)
        {
            var categoryIds = entities
                .Select(q => q.QuestionCategoryId)
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (categoryIds.Count == 0) return;

            var nullableCategoryIds = categoryIds.Select(id => (Guid?)id).ToList();

            // Tìm xem có danh mục con bất kỳ nào có ParentId nằm trong danh sách categoryIds đang xét hay không
            var childCategories = await _categoryService.GetByCondition<QuestionCategory>(c => nullableCategoryIds.Contains(c.ParentId) && !c.IsDeleted);

            if (childCategories.Count > 0)
            {
                var violatedCategoryIds = childCategories
                    .Select(c => c.ParentId!.Value)
                    .Distinct()
                    .ToList();

                var parentCategories = await _categoryService.GetByCondition<QuestionCategory>(c => violatedCategoryIds.Contains(c.QuestionCategoryId));
                var categoryNames = parentCategories.Select(c => c.QuestionCategoryName).ToList();

                var violatedNames = string.Join(", ", categoryNames);
                throw new BadRequestException($"Không thể lưu câu hỏi vào danh mục cha (danh mục chứa danh mục con): {violatedNames}. Chỉ được phép lưu câu hỏi vào danh mục cấp lá.");
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

        #region Override
        protected override async Task AfterInsert(List<Question> entities)
        {
            // Cập nhật 
            foreach (var q in entities)
            {
                q.Answers.ForEach(q => q.QuestionId = q.QuestionId);
            }
            await _answerService.InsertAsync(entities.SelectMany(q => q.Answers).ToList());
        }

        protected override async Task AfterUpdate(List<Question> entities)
        {
            var changeAnswers = new List<QuestionAnswer>();
            
            // Lấy các đáp án cũ cần cập nhật
            changeAnswers.AddRange(entities.SelectMany(e => e.Answers.Where(a => a.QuestionAnswerId != Guid.Empty)));

            // Thêm các đáp án cũ cần xóa (ko có trong danh sách gửi lên)
            var updateIds = changeAnswers.Select(e => e.QuestionAnswerId);
            var questionIds = entities.Select(e => e.QuestionId);
            changeAnswers.AddRange(await _answerService.GetByCondition<QuestionAnswer>(a =>
                questionIds.Contains(a.QuestionId) && !updateIds.Contains(a.QuestionAnswerId)));
            
            // Thêm các đáp án mới
            foreach (var question in entities)
            {

                if (question.QuestionId != Guid.Empty)
                {
                    await _answerService.UpdateAsync(question.Answers);
                }
                else
                {
                    await _answerService.InsertAsync(question.Answers)
                }
                //if (question.Answers != null && question.Answers.Count > 0)
                //{
                    //// 1. Dọn dẹp toàn bộ đáp án cũ của câu hỏi này (nếu có - áp dụng cho cả Update)
                    //var currentAnswers = await _answerService.GetByCondition<QuestionAnswer>(qa => qa.QuestionId == question.QuestionId);
                    //if (currentAnswers.Count > 0)
                    //{
                    //    await _baseWriteDL.DeleteRangeAsync(currentAnswers);
                    //}

                    //// 2. Gán QuestionId, khởi tạo ID và kế thừa thông tin Audit cho các đáp án mới
                    //foreach (var answer in question.Answers)
                    //{
                    //    answer.QuestionAnswerId = answer.QuestionAnswerId == Guid.Empty ? Guid.NewGuid() : answer.QuestionAnswerId;
                    //    answer.QuestionId = question.QuestionId;
                        
                    //    // Kế thừa thông tin Audit từ thực thể Question cha để tránh lỗi CreatedBy null khi INSERT đáp án mới
                    //    answer.CreatedBy = question.ModifiedBy ?? question.CreatedBy ?? "System";
                    //    answer.CreatedDate = question.ModifiedDate ?? (question.CreatedDate == default ? DateTime.UtcNow : question.CreatedDate);
                    //}

                    //// 3. Thêm mới các đáp án vào DB
                    //await _baseWriteDL.InsertRangeAsync(question.Answers);
                //}
            }
        }

        protected override async Task AfterDelete(List<Question> entities)
        {
            foreach (var question in entities)
            {
                // Tự động xóa toàn bộ đáp án liên kết khi câu hỏi bị xóa
                var currentAnswers = await _answerService.GetByCondition<QuestionAnswer>(qa => qa.QuestionId == question.QuestionId);
                if (currentAnswers.Count > 0)
                {
                    await _baseWriteDL.DeleteRangeAsync(currentAnswers);
                }
            }
        }
        #endregion
    }
}


