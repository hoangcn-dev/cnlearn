using HoangCN.Core.BL.Base;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.Common.Utils;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.DL.Interfaces;
using HoangCN.LearnMS.DTOs;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Enums;
using HoangCN.LearnMS.Interfaces;
using HoangCN.LearnMS.Requests;
using System.Text.Json;

namespace HoangCN.LearnMS.Services
{
    /// <summary>
    /// Triển khai dịch vụ câu hỏi trắc nghiệm hỗ trợ import câu hỏi hàng loạt (bọc trong 1 Transaction duy nhất)
    /// </summary>
    public class QuestionService : BaseBL<Question>, IQuestionService
    {
        private readonly ICategoryService _categoryService;
        private readonly IBaseBL<UserSavedMapping> _saveService;
        private readonly ILogger<QuestionService> _logger;

        /// <summary>
        /// Khởi tạo QuestionService với các dependency cần thiết
        /// </summary>
        public QuestionService(
            IBaseReadDL baseReadDL,
            IBaseWriteDL baseWriteDL,
            ICategoryService categoryService,
            ILogger<QuestionService> logger,
            IHttpContextAccessor httpContextAccessor,
            IBaseBL<UserSavedMapping> saveService) : base(baseReadDL, baseWriteDL, httpContextAccessor)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _saveService = saveService;
        }

        public async Task<bool> ValidateOwnership(Guid userId, List<Guid> questionIds)
        {
            var validCount = await CountByCondition(q =>
                questionIds.Contains(q.QuestionId) && q.LearnMsUserId == userId);

            return validCount == questionIds.Count;
        }

        public async Task<QuestionCorrectAnswerMappingDto> GetQuestionCorrectAnswer(Guid? userId, List<Guid> questionIds)
        {
            // Chỉ lấy được đáp án câu hỏi khi nó tồn tại và public | hoặc chỉ có tác giả nếu đang private
            var validQuestions = await GetByCondition<dynamic>(q =>
                questionIds.Contains(q.QuestionId) &&
                (q.AccessType == QuestionAccessType.Public ||
                q.AccessType == QuestionAccessType.Private && q.LearnMsUserId == userId),
                selector: q => new { q.QuestionId, q.CorrectKeys });

            var correctMap = new Dictionary<Guid, List<Guid>>();
            foreach (var q in validQuestions)
            {
                correctMap[q.QuestionId] = JsonSerializer.Deserialize<List<Guid>>(q.CorrectKeys);
            }

            return new QuestionCorrectAnswerMappingDto
            {
                CorrectMap = correctMap
            }; 
        }

        public async Task<List<QuestionAnswerDto>> GetAnswersContent(Guid? userId, List<Guid> questionIds)
        {
            // Chỉ lấy được câu hỏi khi nó tồn tại và public | hoặc chỉ có tác giả nếu đang private
            var validQuestions = await GetByCondition<Question>(q =>
                questionIds.Contains(q.QuestionId) &&
                (q.AccessType == QuestionAccessType.Public ||
                q.AccessType == QuestionAccessType.Private && q.LearnMsUserId == userId));

            var result = new List<QuestionAnswerDto>();
            foreach (var q in validQuestions)
            {
                if (q.Answers != null)
                {
                    foreach (var ans in q.Answers)
                    {
                        result.Add(new QuestionAnswerDto
                        {
                            QuestionAnswerId = ans.QuestionAnswerId,
                            StringContent = ans.StringContent,
                            OrderInList = ans.OrderInList
                        });
                    }
                }
            }
            return result;
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
        /// Lấy danh sách các câu hỏi đã lưu (yêu thích) của một học viên/người dùng cụ thể
        /// </summary>
        /// <param name="userId">Mã định danh của người dùng</param>
        /// <returns>Danh sách DTO chứa thông tin các câu hỏi đã được lưu</returns>
        public async Task<List<SavedDto>> GetSavedQuestions(Guid userId)
        {
            var saveQuestions = await _saveService.GetByCondition<SavedDto>(
                s => s.LearnMsUserId == userId && s.SaveType == SaveType.Question);
            return saveQuestions;
        }

        /// <summary>
        /// Thực hiện bật/tắt (lưu hoặc bỏ lưu) một câu hỏi vào danh sách yêu thích của người dùng
        /// </summary>
        /// <param name="request">Yêu cầu chứa thông tin UserId, TargetId và trạng thái lưu</param>
        public async Task ToggleQuestion(ToggleUserSavedRequest request)
        {
            var mapping = await _saveService.GetFirstByCondition<UserSavedMapping>(
                s => s.LearnMsUserId == request.UserId &&
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
                    LearnMsUserId = request.UserId
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
            ProcessCorrectKeysAndAnswerIds(entities);
            await GenerateSlugForQuestions(entities);
            await _categoryService.ValidateLeafCategories(entities.Select(e => e.QuestionCategoryId).ToList());
            AssignAuthor(entities);
        }

        /// <summary>
        /// Tiền xử lý trước khi cập nhật danh sách câu hỏi
        /// </summary>
        /// <param name="entities">Danh sách câu hỏi cần cập nhật</param>
        protected override async Task HandleBeforeUpdate(List<Question> entities)
        {
            await base.HandleBeforeUpdate(entities);
            await ValidateOwnership(
                    ClaimUtil.GetUserId(_httpContextAccessor.HttpContext?.User)!.Value,
                    entities.Select(q => q.QuestionId).ToList());
            AssignAuthor(entities);
            ProcessCorrectKeysAndAnswerIds(entities);
            await GenerateSlugForQuestions(entities);
            await _categoryService.ValidateLeafCategories(entities.Select(e => e.QuestionCategoryId).ToList());
        }

        /// <summary>
        /// Xử lý sinh Guid cho từng đáp án và gán danh sách CorrectKeys
        /// </summary>
        private void ProcessCorrectKeysAndAnswerIds(List<Question> entities)
        {
            foreach (var question in entities)
            {
                if (question.Answers != null && question.Answers.Count > 0)
                {
                    foreach (var ans in question.Answers)
                    {
                        if (ans.QuestionAnswerId == Guid.Empty)
                        {
                            ans.QuestionAnswerId = Guid.NewGuid();
                        }
                    }

                    var correctAnsIds = question.Answers
                        .Where(a => a.IsCorrectAnswer)
                        .Select(a => a.QuestionAnswerId)
                        .ToList();

                    question.CorrectKeys = correctAnsIds;
                }
                else
                {
                    question.CorrectKeys = [];
                }
            }
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
        /// Tiền xử lý trước khi xóa danh sách câu hỏi
        /// </summary>
        /// <param name="entities">Danh sách câu hỏi cần xóa</param>
        protected override async Task HandleBeforeDelete(List<Question> entities)
        {
            await base.HandleBeforeDelete(entities);

            // Nếu là admin thì bỏ qua
            var role = ClaimUtil.GetRoleName(_httpContextAccessor.HttpContext?.User);
            if (role != RoleNames.Admin.ToString())
            {
                await ValidateOwnership(
                    ClaimUtil.GetUserId(_httpContextAccessor.HttpContext?.User)!.Value,
                    entities.Select(q => q.QuestionId).ToList());
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
        public Guid ExamQuestionId { get; set; }
    }
}


