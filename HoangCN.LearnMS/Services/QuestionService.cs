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
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

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

        /// <summary>
        /// Khởi tạo QuestionService với các dependency cần thiết
        /// </summary>
        /// <param name="baseReadDL">Lớp truy cập dữ liệu đọc (Dapper)</param>
        /// <param name="baseWriteDL">Lớp truy cập dữ liệu ghi (EF Core)</param>
        /// <param name="categoryService">Dịch vụ quản lý danh mục câu hỏi</param>
        /// <param name="logger">Lớp ghi log hệ thống</param>
        /// <param name="httpContextAccessor">Truy cập HttpContext của request hiện tại</param>
        /// <param name="saveService">Dịch vụ lưu trữ câu hỏi yêu thích của user</param>
        /// <param name="answerService">Dịch vụ quản lý đáp án câu hỏi</param>
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

        public async Task<QuestionCorrectAnswerMappingDto> GetQuestionCorrectAnswer(Guid? userId, List<Guid> questionIds)
        {
            // Chỉ lấy được đáp án câu hỏi khi nó tồn tại và public | hoặc chỉ có tác giả nếu đang private
            var validQuestions = await GetByCondition<QuestionIdDto>(q =>
                questionIds.Contains(q.QuestionId) &&
                (q.AccessType == QuestionAccessType.Public ||
                q.AccessType == QuestionAccessType.Private && q.LearnMsUserId == userId));

            var validQuestionIds = validQuestions.Select(q => q.QuestionId).ToList();

            var answers = await _answerService.GetByCondition<QuestionAnswerDto>(a =>
                validQuestionIds.Contains(a.QuestionId) &&
                a.IsCorrectAnswer);

            return new QuestionCorrectAnswerMappingDto
            {
                CorrectMap = answers
                    .GroupBy(a => a.QuestionId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(a => a.QuestionAnswerId).ToList()
                    )
            }; 
        }

        public async Task<List<QuestionAnswerDto>> GetAnswersContent(Guid? userId, List<Guid> questionIds)
        {
            // Chỉ lấy được câu hỏi khi nó tồn tại và public | hoặc chỉ có tác giả nếu đang private
            var validQuestions = await GetByCondition<QuestionIdDto>(q =>
                questionIds.Contains(q.QuestionId) &&
                (q.AccessType == QuestionAccessType.Public ||
                q.AccessType == QuestionAccessType.Private && q.LearnMsUserId == userId));

            var validQuestionIds = validQuestions.Select(q => q.QuestionId).ToList();

            var answers = await _answerService.GetByCondition<QuestionAnswerDto>(a => 
                validQuestionIds.Contains(a.QuestionId));
            return answers;
        }

        public async Task<QuestionDto> GetQuestionContent(Guid? userId, Guid questionId)
        {
            // Chỉ lấy được câu hỏi khi nó tồn tại và public | hoặc chỉ có tác giả nếu đang private
            var qContent = await GetFirstByCondition<QuestionDto>(q => 
                q.QuestionId == questionId && 
                (q.AccessType == QuestionAccessType.Public ||
                q.AccessType == QuestionAccessType.Private && q.LearnMsUserId == userId));

            if (qContent == null)
            {
                throw new BadRequestException("Câu hỏi không tồn tại hoặc bị giới hạn quyền truy cập");
            }

            return qContent;
        }

        /// <summary>
        /// Lấy chi tiết một câu hỏi trong ngân hàng đề kèm theo toàn bộ đáp án của nó sử dụng Multiple Query
        /// </summary>
        /// <param name="questionId">Mã định danh của câu hỏi</param>
        /// <returns>Đối tượng DTO chứa thông tin câu hỏi và danh sách đáp án liên quan</returns>
        public async Task<BankQuestionWithAnswersDto> GetBankQuestionWithAnswers(Guid questionId)
        {
            var param = new DynamicParameters();

            // Tạo câu sql lấy thông tin câu hỏi
            var questionQuery = BuildSQLUtil.BuildQueryStringGetDtoByCondition<Question, QuestionDto>(
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

        /// <summary>
        /// Lấy danh sách các câu hỏi đã lưu (yêu thích) của một học viên/người dùng cụ thể
        /// </summary>
        /// <param name="userId">Mã định danh của người dùng</param>
        /// <returns>Danh sách DTO chứa thông tin các câu hỏi đã được lưu</returns>
        public async Task<List<SavedDto>> GetSavedQuestions(Guid userId)
        {
            var saveQuestions = await _saveService.GetByCondition<SavedDto>(
                s => s.UserId == userId && s.SaveType == SaveType.Question);
            return saveQuestions;
        }

        /// <summary>
        /// Thực hiện bật/tắt (lưu hoặc bỏ lưu) một câu hỏi vào danh sách yêu thích của người dùng
        /// </summary>
        /// <param name="request">Yêu cầu chứa thông tin UserId, TargetId và trạng thái lưu</param>
        public async Task ToggleQuestion(ToggleUserSavedRequest request)
        {
            var mapping = await _saveService.GetFirstByCondition<UserSavedMapping>(
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
                await _saveService.InsertEntities([new () {
                    TargetId = request.TargetId,
                    SaveType = SaveType.Question,
                    UserId = request.UserId
                }]);
            }
            else
            {
                await _saveService.DeleteEntities(new DeleteRequest()
                {
                    Ids = [ mapping!.UserSavedMappingId ]
                });
            }
            
        }

        #region Override
        /// <summary>
        /// Tiền xử lý trước khi thêm mới danh sách câu hỏi
        /// </summary>
        /// <param name="entities">Danh sách câu hỏi cần thêm mới</param>
        protected override async Task HandleBeforeInsert(List<Question> entities)
        {
            await base.HandleBeforeInsert(entities);
            GenerateSlugForQuestions(entities);
            await ValidateLeafCategories(entities);
            AssignAuthor(entities);
        }

        /// <summary>
        /// Tiền xử lý trước khi cập nhật danh sách câu hỏi
        /// </summary>
        /// <param name="entities">Danh sách câu hỏi cần cập nhật</param>
        protected override async Task HandleBeforeUpdate(List<Question> entities)
        {
            await base.HandleBeforeUpdate(entities);
            GenerateSlugForQuestions(entities);
            await ValidateLeafCategories(entities);
        }

        /// <summary>
        /// Gán thông tin người dùng sở hữu (tác giả) cho các câu hỏi dựa trên token đăng nhập
        /// </summary>
        /// <param name="questions">Danh sách câu hỏi đang xử lý</param>
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

        /// <summary>
        /// Tự động sinh SEO slug thân thiện cho câu hỏi dựa vào nội dung văn bản
        /// </summary>
        /// <param name="entities">Danh sách câu hỏi cần sinh slug</param>
        private async Task GenerateSlugForQuestions(List<Question> entities)
        {
            foreach (var entity in entities)
            {
                if (entity == null) continue;

                if (string.IsNullOrWhiteSpace(entity.StringContent))
                {
                    throw new BadRequestException("Nội dung câu hỏi không được phép để trống.");
                }
                entity.QuestionSlug = SlugUtil.GenerateSlug(entity.StringContent);
            }
        }

        /// <summary>
        /// Kiểm tra và đảm bảo các câu hỏi chỉ được lưu vào danh mục cấp lá (không chứa danh mục con)
        /// </summary>
        /// <param name="entities">Danh sách câu hỏi cần kiểm tra danh mục</param>
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

        #endregion
    }

    /// <summary>
    /// DTO nội bộ dùng để tối ưu hóa truy vấn lấy ID câu hỏi
    /// </summary>
    internal class QuestionIdDto
    {
        public Guid QuestionId { get; set; }
    }
}


