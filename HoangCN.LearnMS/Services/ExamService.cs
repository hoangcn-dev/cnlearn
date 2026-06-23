using HoangCN.Core.BL.Base;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Utils;
using HoangCN.Core.DL.Interfaces;
using HoangCN.LearnMS.DTOs;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Enums;
using HoangCN.LearnMS.Interfaces;
using System.Text.Json;

namespace HoangCN.LearnMS.Services
{
    /// <summary>
    /// Triển khai nghiệp vụ quản lý đề thi
    /// </summary>
    public class ExamService : BaseBL<Exam>, IExamService
    {
        private readonly IQuestionService _questionService;
        private readonly ICategoryService _categoryService;
        private readonly IBaseBL<ExamQuestion> _examQuestionService;

        public ExamService(
            IBaseReadDL baseReadDL,
            IBaseWriteDL baseWriteDL,
            IQuestionService questionService,
            IBaseBL<ExamQuestion> examQuestionBL,
            IHttpContextAccessor httpContextAccessor,
            ICategoryService categoryService) : base(baseReadDL, baseWriteDL, httpContextAccessor)
        {
            _questionService = questionService ?? throw new ArgumentNullException(nameof(questionService));
            _examQuestionService = examQuestionBL ?? throw new ArgumentNullException(nameof(examQuestionBL));
            _categoryService = categoryService;
        }

        public async Task<QuestionCorrectAnswerMappingDto> GetExamCorrectKeys(Guid? userId, Guid examId)
        {
            // Chỉ lấy được đáp án câu hỏi khi nó tồn tại và public | hoặc chỉ có tác giả nếu đang private
            var validCount = await CountByCondition(e =>
                e.ExamId == examId && (
                e.AccessType == ExamAccessType.Public || e.AccessType == ExamAccessType.Private && e.LearnMsUserId == userId));

            if (validCount != 1)
            {
                throw new BadRequestException("Đề thi không tồn tại hoặc bị giới hạn quyền truy cập"); 
            }

            var questions = await _examQuestionService.GetByCondition<dynamic>(
                q => q.ExamId == examId,
                selector: e => new { e.ExamQuestionId, e.CorrectKeys });

            var correctMap = new Dictionary<Guid, List<Guid>>();
            foreach (var q in questions)
            {
                correctMap[q.ExamQuestionId] = JsonSerializer.Deserialize<List<Guid>>(q.CorrectKeys);
            }

            return new QuestionCorrectAnswerMappingDto
            {
                CorrectMap = correctMap
            };
        }

        /// <summary>
        /// Lấy chi tiết đề thi có kiểm tra quyền truy cập riêng tư
        /// </summary>
        public async Task<ExamDto> GetExamByIdAsync(Guid? userId, Guid examId)
        {
            var exam = await GetFirstByCondition<ExamDto>(e =>
                e.ExamId == examId &&
                (e.AccessType == ExamAccessType.Public || e.AccessType == ExamAccessType.Private && e.LearnMsUserId == userId));

            if (exam == null)
            {
                throw new NotFoundException("Đề thi không tồn tại hoặc bị giới hạn quyền truy cập");
            }

            return exam;
        }

        /// <summary>
        /// Lấy danh sách câu hỏi thuộc đề thi và xếp đúng thứ tự
        /// </summary>
        public async Task<List<ExamQuestionDto>> GetExamQuestionsAsync(Guid? userId, Guid examId)
        {
            var count = await CountByCondition(e =>
                e.ExamId == examId &&
                (e.AccessType == ExamAccessType.Public || e.AccessType == ExamAccessType.Private && e.LearnMsUserId == userId));

            if (count == 0)
            {
                throw new NotFoundException("Đề thi không tồn tại hoặc bị giới hạn quyền truy cập");
            }

            // Lấy danh sách câu hỏi
            var questions = await _examQuestionService.GetByCondition<ExamQuestionDto>(e =>
                e.ExamId == examId);

            return questions;
        }

        public async Task<List<QuestionAnswerDto>> GetAnswersContent(Guid? userId, Guid examId)
        {
            // Chỉ lấy được đáp án câu hỏi khi nó tồn tại và public | hoặc chỉ có tác giả nếu đang private
            var validCount = await CountByCondition(e =>
                e.ExamId == examId && (
                e.AccessType == ExamAccessType.Public || e.AccessType == ExamAccessType.Private && e.LearnMsUserId == userId));

            if (validCount != 1)
            {
                throw new BadRequestException("Đề thi không tồn tại hoặc bị giới hạn quyền truy cập");
            }

            var validQuestions = await _examQuestionService.GetByCondition<ExamQuestion>(q =>
                q.ExamId == examId);

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

        #region Override

        protected override async Task HandleBeforeInsert(List<Exam> entities)
        {
            await base.HandleBeforeInsert(entities);

            // Kiểm tra danh mục lá
            await _categoryService.ValidateLeafCategories(entities.Select(e => e.QuestionCategoryId).ToList());

            // Assign tác giả
            entities.ForEach(e =>
            {
                e.LearnMsUserId = ClaimUtil.GetUserId(_httpContextAccessor.HttpContext.User)!.Value;
            });

            // Đảm báo trạng thái insert cho toàn bộ câu hỏi đi kèm
            var questions = entities.SelectMany(e => e.Questions);
            foreach (var item in questions)
            {
                item.State = ModalState.Insert;
            }

            ProcessExamQuestions(entities);
        }

        protected override async Task HandleBeforeUpdate(List<Exam> entities)
        {
            await base.HandleBeforeUpdate(entities);
            await _categoryService.ValidateLeafCategories(entities.Select(e => e.QuestionCategoryId).ToList());
            // Assign tác giả
            entities.ForEach(e =>
            {
                e.LearnMsUserId = ClaimUtil.GetUserId(_httpContextAccessor.HttpContext.User)!.Value;
            });
            await ValidateOwnership(entities);
            ProcessExamQuestions(entities);
        }

        /// <summary>
        /// Xử lý sinh Guid cho các câu hỏi và đáp án, gán CorrectKeys dạng List<Guid>
        /// </summary>
        private void ProcessExamQuestions(List<Exam> entities)
        {
            foreach (var exam in entities)
            {
                if (exam.Questions != null)
                {
                    foreach (var eq in exam.Questions)
                    {
                        if (eq.ExamQuestionId == Guid.Empty)
                        {
                            eq.ExamQuestionId = Guid.NewGuid();
                        }

                        if (eq.Answers != null && eq.Answers.Count > 0)
                        {
                            foreach (var ans in eq.Answers)
                            {
                                if (ans.QuestionAnswerId == Guid.Empty)
                                {
                                    ans.QuestionAnswerId = Guid.NewGuid();
                                }
                            }

                            var correctAnsIds = eq.Answers
                                .Where(a => a.IsCorrectAnswer)
                                .Select(a => a.QuestionAnswerId)
                                .ToList();

                            eq.CorrectKeys = correctAnsIds;
                        }
                        else
                        {
                            eq.CorrectKeys = [];
                        }
                    }
                }
            }
        }

        protected override async Task HandleBeforeDelete(List<Exam> entities)
        {
            await base.HandleBeforeDelete(entities);
            await ValidateOwnership(entities);
        }

        #endregion

        #region Internal
        
        private async Task ValidateOwnership(List<Exam> exams)
        {
            var userId = ClaimUtil.GetUserId(_httpContextAccessor.HttpContext?.User);
            if (userId == null) throw new UnauthorizedException("Vui lòng đăng nhập để tiếp tục");

            var examIds = exams.Select(e => e.ExamId);
            var validCount = await CountByCondition(e => examIds.Contains(e.ExamId) && e.LearnMsUserId == userId);

            if (validCount != exams.Count)
            {
                throw new ForbiddenException("Bạn không có quyền chỉnh sửa 1 trong số đề thi này");
            }
        }



        //private async Task ProcessExamQuestionsContribute(List<Exam> exams)
        //{
        //    var userId = ClaimUtil.GetUserId(_httpContextAccessor.HttpContext?.User);
        //    if (userId == null) throw new UnauthorizedException("Vui lòng đăng nhập để tiếp tục");
        //    var newQuestions = new List<Question>();
        //    foreach (var exam in exams)
        //    {
        //        exam.ExamId = Guid.NewGuid();
        //        exam.LearnMsUserId = userId.Value;
        //        exam.State = ModalState.Insert;

        //        // Mỗi đề đều tạo câu hỏi mới (lấy thông tin được gửi kèm lên)
        //        for (int i = 0; i < exam.Questions.Count; i++)
        //        {
        //            var eq = exam.Questions[i];

        //            eq.Question.QuestionId = Guid.NewGuid();
        //            eq.Question.QuestionSlug = SlugUtil.GenerateSlug(eq.Question.StringContent);
        //            eq.Question.ExamId = exam.ExamId;
        //            eq.Question.State = ModalState.Insert;
        //            eq.Question.Source = exam.Name;
        //            eq.Question.Answers.ForEach(e => e.QuestionAnswerId = Guid.Empty);
        //            eq.Question.AccessType = exam.ContributeToBank?
        //                QuestionAccessType.Public : QuestionAccessType.Private;
        //            newQuestions.Add(eq.Question);

        //            eq.QuestionId = eq.Question.QuestionId;
        //            eq.SortOrder = i + 1;
        //        }
        //    }

        //    // Lưu câu hỏi mới
        //    await _questionService.InsertEntities(newQuestions);
        //}

        #endregion
    }
}
