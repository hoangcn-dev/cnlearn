using HoangCN.Core.BL.Base;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.BL.Utils;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.LearnMS.Entities;
using HoangCN.Core.Common.Utils;
using HoangCN.Core.DL.Interfaces;
using HoangCN.LearnMS.Interfaces;
using HoangCN.LearnMS.DTOs;
using HoangCN.LearnMS.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using HoangCN.LearnMS.Requests;
using HoangCN.LearnMS.Enums;

using Microsoft.AspNetCore.Http;

namespace HoangCN.LearnMS.Services
{
    /// <summary>
    /// Triển khai dịch vụ câu hỏi trắc nghiệm hỗ trợ import câu hỏi hàng loạt (bọc trong 1 Transaction duy nhất)
    /// </summary>
    public class QuestionService : BaseBL<Question>, IQuestionService
    {
        private readonly IBaseBL<QuestionCategory> _categoryService;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(
            IBaseReadDL baseReadDL, 
            IBaseWriteDL baseWriteDL,
            IBaseBL<QuestionCategory> categoryService, 
            ILogger<QuestionService> logger,
            IHttpContextAccessor httpContextAccessor) : base(baseReadDL, baseWriteDL, httpContextAccessor)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                if (string.IsNullOrWhiteSpace(entity.Slug))
                {
                    if (string.IsNullOrWhiteSpace(entity.StringContent))
                    {
                        throw new BadRequestException("Nội dung câu hỏi không được phép để trống.");
                    }
                    entity.Slug = SlugUtil.GenerateSlug(entity.StringContent);
                }
                else
                {
                    entity.Slug = SlugUtil.GenerateSlug(entity.Slug);
                }
            }
        }



        /// <summary>
        /// Tiền xử lý xóa - thực hiện xóa mềm/cascade các thực thể liên quan
        /// </summary>
        public override async Task DeleteAsync(DeleteRequest request)
        {
            var res = await Get<Question>(new GetRequest { Ids = request.Ids });
            if (res.Items.Count == 0) return;

            var entities = res.Items;
            var questionIds = entities.Select(q => q.QuestionId).ToList();

            var queryableAns = _baseWriteDL.GetQueryable<QuestionAnswer>();

            // 1. Xóa mềm đáp án
            var answers = queryableAns
                .Where(a => questionIds.Contains(a.QuestionId) && !a.IsDeleted)
                .ToList();
            foreach (var a in answers)
            {
                a.IsDeleted = true;
            }

            await _baseWriteDL.BeginTransactionAsync();
            try
            {
                if (answers.Count > 0)
                {
                    await _baseWriteDL.DeleteRangeAsync(answers);
                }
                await _baseWriteDL.DeleteRangeAsync(entities);
                await _baseWriteDL.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _baseWriteDL.RollbackTransactionAsync();
                _logger.LogError(ex, "Thất bại khi thực thi Delete. Đã rollback.");
                throw;
            }
        }

        /// <summary>
        /// Chấm điểm và trả về kết quả đáp án cho một câu hỏi
        /// </summary>
        public async Task<QuestionCheckResultDto> CheckAnswerAsync(QuestionCheckDto dto)
        {
            var q = await GetById<Question>(dto.QuestionId);
            if (q == null)
            {
                throw new BadRequestException("Không tìm thấy câu hỏi.");
            }

            var ansSql = QuestionSqlUtil.BuildQueryAnswersByQuestionId(dto.QuestionId, out var ansParams);
            var answers = await _baseReadDL.ExecuteQueryText<QuestionAnswer>(ansSql, ansParams);

            var correctIds = answers.Where(a => a.IsCorrectAnswer).Select(a => a.QuestionAnswerId).ToList();
            
            var selectedIds = dto.SelectedAnswerIds ?? new List<Guid>();

            bool isCorrect = true;

            // Kiểm tra số lượng đáp án chọn có khớp số lượng đáp án đúng không
            if (selectedIds.Count != correctIds.Count)
            {
                isCorrect = false;
            }
            else
            {
                // Nếu khớp số lượng, kiểm tra từng đáp án chọn xem có nằm trong danh sách đúng không
                foreach (var id in selectedIds)
                {
                    if (!correctIds.Contains(id))
                    {
                        isCorrect = false;
                        break;
                    }
                }
            }

            return new QuestionCheckResultDto
            {
                IsCorrect = isCorrect,
                Explanation = q.Explaination,
                CorrectAnswerIds = correctIds
            };
        }

        /// <summary>
        /// Lấy danh sách câu hỏi phân trang kèm chi tiết đáp án và danh mục
        /// </summary>
        public async Task<ResultDto<QuestionDetailsDto>> GetQuestionDetailsPagingAsync(GetRequest request, Guid currentUserId)
        {
            request ??= new GetRequest();
            request.Filters ??= new List<Filter>();
            request.Ids ??= new List<Guid>();

            var result = new ResultDto<QuestionDetailsDto>
            {
                Page = request.Page ?? 1,
                Size = request.Size ?? 10,
                Total = 0,
                Items = new List<QuestionDetailsDto>()
            };

            Expression<Func<Question, bool>>? condition = q => q.AccessType == QuestionAccessType.Public || q.UserId == currentUserId;

            // Xử lý bộ lọc IsMyCreated
            var myCreatedFilter = request.Filters.FirstOrDefault(f => f != null && string.Equals(f.Property, "IsMyCreated", StringComparison.OrdinalIgnoreCase));
            if (myCreatedFilter != null)
            {
                request.Filters.Remove(myCreatedFilter);
                if (bool.TryParse(myCreatedFilter.Value?.ToString(), out bool isMyCreated) && isMyCreated)
                {
                    condition = q => q.UserId == currentUserId;
                }
            }
            // Xử lý bộ lọc IsSaved
            var savedFilter = request.Filters.FirstOrDefault(f => f != null && string.Equals(f.Property, "IsSaved", StringComparison.OrdinalIgnoreCase));
            if (savedFilter != null)
            {
                request.Filters.Remove(savedFilter);

                var savedSql = QuestionSqlUtil.BuildQuerySavedQuestionIds(currentUserId, out var savedParams);
                var savedQuestionIds = await _baseReadDL.ExecuteQueryText<Guid>(savedSql, savedParams);
                var savedQuestionIdsList = (savedQuestionIds ?? Enumerable.Empty<Guid>()).ToList();

                if (savedQuestionIdsList.Count == 0)
                {
                    return result; // Trả về danh sách rỗng nếu không có câu hỏi nào được lưu
                }

                if (request.Ids.Count > 0)
                {
                    request.Ids = request.Ids.Intersect(savedQuestionIdsList).ToList();
                }
                else
                {
                    request.Ids = savedQuestionIdsList;
                }

                if (request.Ids.Count == 0)
                {
                    return result; // Trả về danh sách rỗng sau khi giao nhau
                }
            }

            // Xử lý bộ lọc IsDone
            var doneFilter = request.Filters.FirstOrDefault(f => f != null && string.Equals(f.Property, "IsDone", StringComparison.OrdinalIgnoreCase));
            if (doneFilter != null)
            {
                request.Filters.Remove(doneFilter);

                var doneSql = QuestionSqlUtil.BuildQueryDoneQuestionIds(currentUserId, out var doneParams);
                var doneQuestionIds = await _baseReadDL.ExecuteQueryText<Guid>(doneSql, doneParams);
                var doneQuestionIdsList = (doneQuestionIds ?? Enumerable.Empty<Guid>()).ToList();

                if (doneQuestionIdsList.Count == 0)
                {
                    return result; // Trả về danh sách rỗng nếu không có câu hỏi nào được làm
                }

                if (request.Ids.Count > 0)
                {
                    request.Ids = request.Ids.Intersect(doneQuestionIdsList).ToList();
                }
                else
                {
                    request.Ids = doneQuestionIdsList;
                }

                if (request.Ids.Count == 0)
                {
                    return result; // Trả về danh sách rỗng sau khi giao nhau
                }
            }

            // Xử lý bộ lọc CategoryId: Đổi tên thuộc tính để khớp với trường QuestionCategoryId trên bảng Question
            var categoryFilter = request.Filters.FirstOrDefault(f => f != null && string.Equals(f.Property, "CategoryId", StringComparison.OrdinalIgnoreCase));
            if (categoryFilter != null)
            {
                categoryFilter.Property = "QuestionCategoryId";
            }

            var questionsResult = await Get<Question>(request, condition);
            if (questionsResult == null)
            {
                return result;
            }

            result.Page = questionsResult.Page;
            result.Size = questionsResult.Size;
            result.Total = questionsResult.Total;

            if (questionsResult.Items == null || questionsResult.Items.Count == 0)
            {
                return result;
            }

            var questionIds = questionsResult.Items
                .Where(q => q != null)
                .Select(q => q.QuestionId)
                .ToList();

            if (questionIds.Count == 0)
            {
                return result;
            }

            // Lấy danh sách đáp án qua raw SQL tập trung
            var ansSql = QuestionSqlUtil.BuildQueryAnswersByQuestionIds(questionIds, out var ansParams);
            var answers = await _baseReadDL.ExecuteQueryText<QuestionAnswer>(ansSql, ansParams);

            var answerGroup = (answers ?? Enumerable.Empty<QuestionAnswer>())
                .Where(a => a != null)
                .GroupBy(a => a.QuestionId)
                .ToDictionary(g => g.Key, g => g.OrderBy(a => a.OrderInList).ToList());

            foreach (var q in questionsResult.Items)
            {
                if (q == null) continue;

                answerGroup.TryGetValue(q.QuestionId, out var qAnswers);

                result.Items.Add(new QuestionDetailsDto
                {
                    Id = q.QuestionId,
                    Slug = q.Slug,
                    StringContent = q.StringContent,
                    Explanation = q.Explaination,
                    Level = (int)q.Level,
                    Type = (int)q.Type,
                    AccessType = (int)q.AccessType,
                    IsMyCreated = q.UserId == currentUserId,
                    QuestionCategoryId = q.QuestionCategoryId,
                    Answers = qAnswers?.Select(a => new AnswerDetailsDto
                    {
                        QuestionAnswerId = a.QuestionAnswerId,
                        StringContent = a.StringContent,
                        IsCorrectAnswer = a.IsCorrectAnswer
                    }).ToList() ?? new List<AnswerDetailsDto>()
                });
            }

            return result;
        }


        /// <summary>
        /// Lấy chi tiết câu hỏi theo ID
        /// </summary>
        public async Task<QuestionDetailsDto?> GetQuestionDetailsByIdAsync(Guid id, Guid currentUserId)
        {
            var q = await GetById<Question>(id);
            if (q == null) return null;

            // Lấy danh sách đáp án qua raw SQL tập trung
            var ansSql = QuestionSqlUtil.BuildQueryAnswersByQuestionId(id, out var ansParams);
            var answers = await _baseReadDL.ExecuteQueryText<QuestionAnswer>(ansSql, ansParams);

            int correctCount = answers?.Count(a => a.IsCorrectAnswer) ?? 0;
            int determinedType = correctCount > 1 ? 1 : 0;

            return new QuestionDetailsDto
            {
                Id = q.QuestionId,
                Slug = q.Slug,
                StringContent = q.StringContent,
                Explanation = q.Explaination,
                Level = (int)q.Level,
                Type = determinedType,
                AccessType = (int)q.AccessType,
                IsMyCreated = q.UserId == currentUserId,
                QuestionCategoryId = q.QuestionCategoryId,
                Answers = answers.Select(a => new AnswerDetailsDto
                {
                    QuestionAnswerId = a.QuestionAnswerId,
                    StringContent = a.StringContent,
                    IsCorrectAnswer = a.IsCorrectAnswer
                }).ToList()
            };
        }

        /// <summary>
        /// Lấy danh sách câu hỏi thuộc một đề thi (bỏ qua điều kiện AccessType để đảm bảo học viên thấy nội dung đề thi)
        /// </summary>
        public async Task<List<QuestionDetailsDto>> GetQuestionsByExamIdAsync(Guid examId, Guid currentUserId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("ExamId", examId);

            var sql = @"
                SELECT q.* 
                FROM Question q
                INNER JOIN ExamQuestion eq ON q.QuestionId = eq.QuestionId
                WHERE eq.ExamId = @ExamId AND q.IsDeleted = 0 AND eq.IsDeleted = 0
                ORDER BY eq.SortOrder";

            var questions = (await _baseReadDL.ExecuteQueryText<Question>(sql, parameters))?.ToList();
            if (questions == null || questions.Count == 0)
            {
                return new List<QuestionDetailsDto>();
            }

            var questionIds = questions.Select(q => q.QuestionId).ToList();

            var ansSql = QuestionSqlUtil.BuildQueryAnswersByQuestionIds(questionIds, out var ansParams);
            var answers = await _baseReadDL.ExecuteQueryText<QuestionAnswer>(ansSql, ansParams);

            var answerGroup = (answers ?? Enumerable.Empty<QuestionAnswer>())
                .GroupBy(a => a.QuestionId)
                .ToDictionary(g => g.Key, g => g.OrderBy(a => a.OrderInList).ToList());

            var result = new List<QuestionDetailsDto>();
            foreach (var q in questions)
            {
                answerGroup.TryGetValue(q.QuestionId, out var qAnswers);

                bool isOwner = q.UserId == currentUserId;
                int correctCount = qAnswers?.Count(a => a.IsCorrectAnswer) ?? 0;
                int determinedType = correctCount > 1 ? 1 : 0;

                result.Add(new QuestionDetailsDto
                {
                    Id = q.QuestionId,
                    Slug = q.Slug,
                    StringContent = q.StringContent,
                    Explanation = isOwner ? q.Explaination : null,
                    Level = (int)q.Level,
                    Type = determinedType,
                    AccessType = (int)q.AccessType,
                    IsMyCreated = isOwner,
                    QuestionCategoryId = q.QuestionCategoryId,
                    Answers = qAnswers?.Select(a => new AnswerDetailsDto
                    {
                        QuestionAnswerId = a.QuestionAnswerId,
                        StringContent = a.StringContent,
                        IsCorrectAnswer = isOwner ? a.IsCorrectAnswer : false
                    }).ToList() ?? new List<AnswerDetailsDto>()
                });
            }

            return result;
        }

        /// <summary>
        /// Lưu danh sách câu hỏi chi tiết (Thêm mới/Cập nhật) kèm đáp án và danh mục
        /// </summary>
        public async Task SaveQuestionDetailsAsync(List<QuestionDetailsDto> questionsDto, Guid currentUserId)
        {
            if (questionsDto == null || questionsDto.Count == 0)
            {
                throw new BadRequestException("Dữ liệu trống hoặc sai định dạng");
            }

            // Validate dữ liệu đầu vào
            ValidateUtil.CommonValidate(questionsDto);

            // Kiểm tra tồn tại danh mục câu hỏi (CheckExist)
            await ValidateUtil.CheckExist(questionsDto, _baseReadDL);

            var questionsToInsert = new List<Question>();
            var questionsToUpdate = new List<Question>();
            var answersToInsert = new List<QuestionAnswer>();
            var answersToUpdate = new List<QuestionAnswer>();
            var queryableAns = _baseWriteDL.GetQueryable<QuestionAnswer>();

            foreach (var qDto in questionsDto)
            {
                var isNew = qDto.Id == Guid.Empty;
                var questionId = isNew ? Guid.NewGuid() : qDto.Id;
                
                if (string.IsNullOrWhiteSpace(qDto.StringContent))
                {
                    throw new BadRequestException("Nội dung câu hỏi không được phép để trống.");
                }

                var slug = string.IsNullOrWhiteSpace(qDto.Slug) 
                    ? SlugUtil.GenerateSlug(qDto.StringContent) 
                    : SlugUtil.GenerateSlug(qDto.Slug);

                Question q;
                bool isInsertingQuestion = false;
                if (isNew)
                {
                    q = new Question
                    {
                        QuestionId = questionId,
                        UserId = currentUserId
                    };
                    questionsToInsert.Add(q);
                    isInsertingQuestion = true;
                }
                else
                {
                    var existingList = await GetByCondition<Question>(x => x.QuestionId == questionId);
                    q = existingList.FirstOrDefault()!;
                    if (q == null)
                    {
                        q = new Question
                        {
                            QuestionId = questionId,
                            UserId = currentUserId
                        };
                        questionsToInsert.Add(q);
                        isInsertingQuestion = true;
                    }
                    else
                    {
                        questionsToUpdate.Add(q);
                    }
                }

                q.Slug = slug;
                q.StringContent = qDto.StringContent?.Trim();
                q.Explaination = qDto.Explanation?.Trim();
                q.Level = (QuestionLevel)qDto.Level;
                q.Type = (QuestionType)qDto.Type;
                q.AccessType = (QuestionAccessType)qDto.AccessType;
                q.QuestionCategoryId = qDto.QuestionCategoryId;
                q.IsInBank = false;

                // Load answers hiện tại
                var existingAnswers = new List<QuestionAnswer>();
                if (!isInsertingQuestion)
                {
                    existingAnswers = queryableAns
                        .Where(a => a.QuestionId == questionId && !a.IsDeleted)
                        .ToList();
                }

                // Xóa mềm đáp án cũ không còn trong danh sách mới
                var inputAnswerIds = qDto.Answers.Select(a => a.QuestionAnswerId).Where(id => id != Guid.Empty).ToList();
                foreach (var ea in existingAnswers)
                {
                    if (!inputAnswerIds.Contains(ea.QuestionAnswerId))
                    {
                        ea.IsDeleted = true;
                        answersToUpdate.Add(ea);
                    }
                }

                // Thêm hoặc cập nhật đáp án hiện tại
                for (int i = 0; i < qDto.Answers.Count; i++)
                {
                    var ansDto = qDto.Answers[i];
                    var isNewAns = isInsertingQuestion || ansDto.QuestionAnswerId == Guid.Empty;
                    QuestionAnswer ans;
                    if (isNewAns)
                    {
                        ans = new QuestionAnswer
                        {
                            QuestionAnswerId = Guid.NewGuid(),
                            QuestionId = questionId
                        };
                        answersToInsert.Add(ans);
                    }
                    else
                    {
                        ans = existingAnswers.FirstOrDefault(a => a.QuestionAnswerId == ansDto.QuestionAnswerId);
                        if (ans == null)
                        {
                            ans = new QuestionAnswer
                            {
                                QuestionAnswerId = ansDto.QuestionAnswerId,
                                QuestionId = questionId
                            };
                            answersToInsert.Add(ans);
                        }
                        else
                        {
                            answersToUpdate.Add(ans);
                        }
                    }

                    ans.StringContent = ansDto.StringContent?.Trim();
                    ans.IsCorrectAnswer = ansDto.IsCorrectAnswer;
                    ans.OrderInList = i + 1;
                }
            }

            // Ghi nhận thay đổi trong Transaction
            await _baseWriteDL.BeginTransactionAsync();
            try
            {
                var now = DateTime.Now;

                // Điền thông tin Audit
                foreach (var q in questionsToInsert)
                {
                    q.CreatedBy = "Hoàng Cao Nguyên";
                    q.CreatedDate = now;
                    q.ModifiedBy = "Hoàng Cao Nguyên";
                    q.ModifiedDate = now;
                }
                foreach (var q in questionsToUpdate)
                {
                    q.ModifiedBy = "Hoàng Cao Nguyên";
                    q.ModifiedDate = now;
                }

                foreach (var ans in answersToInsert)
                {
                    ans.CreatedBy = "Hoàng Cao Nguyên";
                    ans.CreatedDate = now;
                    ans.ModifiedBy = "Hoàng Cao Nguyên";
                    ans.ModifiedDate = now;
                }
                foreach (var ans in answersToUpdate)
                {
                    ans.ModifiedBy = "Hoàng Cao Nguyên";
                    ans.ModifiedDate = now;
                }

                // Thực thi ghi DB
                if (questionsToInsert.Count > 0) await _baseWriteDL.InsertRangeAsync(questionsToInsert);
                if (questionsToUpdate.Count > 0) await _baseWriteDL.UpdateRangeAsync(questionsToUpdate);
                if (answersToInsert.Count > 0) await _baseWriteDL.InsertRangeAsync(answersToInsert);
                if (answersToUpdate.Count > 0) await _baseWriteDL.UpdateRangeAsync(answersToUpdate);

                await _baseWriteDL.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _baseWriteDL.RollbackTransactionAsync();
                _logger.LogError(ex, "Thất bại khi thực thi SaveQuestionDetails. Đã rollback.");
                throw;
            }
        }

        /// <summary>
        /// Lưu danh sách câu hỏi 
        /// </summary>
        public async Task SaveListQuestions(SaveQuestionsRequest request, Guid currentUserId)
        {
            // Đảm bảo có dữ liệu để lưu
            if (request.Questions.Count == 0)
            {
                throw new BadRequestException("Dữ liệu trống hoặc sai định dạng");
            }

            // Validate đơn giản
            ValidateUtil.CommonValidate(request.Questions);

            // Kiểm tra tồn tại
            await ValidateUtil.CheckExist(request.Questions, _baseReadDL);

            // Đảm bảo mỗi câu hỏi có ít nhất 1 đáp án đúng
            foreach (var q in request.Questions)
            {
                if (q.Answers == null || q.Answers.Count == 0 || !q.Answers.Any(a => a.IsCorrectAnswer))
                {
                    throw new BadRequestException("Mỗi câu hỏi phải có ít nhất một đáp án đúng.");
                }
            }

            // Tạo danh sách thực thể để lưu
            var questionsToInsert = new List<Question>();
            var answersToInsert = new List<QuestionAnswer>();
            Func<SaveQuestionsDto, QuestionType> getQuestionType = (q) =>
            {
                var correctCount = q.Answers.Count(a => a.IsCorrectAnswer);
                if (correctCount == 0)
                {
                    throw new BadRequestException("Mỗi câu hỏi phải có ít nhất một đáp án đúng.");
                }
                else if (correctCount == 1)
                {
                    return QuestionType.SingleChoice;
                }
                else
                {
                    return QuestionType.MultipleChoice;
                }
            };
            foreach (var q in request.Questions)
            {
                questionsToInsert.Add(new Question
                {
                    QuestionId = Guid.NewGuid(),
                    Slug = SlugUtil.GenerateSlug(q.StringContent.Trim()),
                    StringContent = q.StringContent?.Trim(),
                    Explaination = q.Explaination?.Trim(),
                    Level = q.Level,
                    Type = getQuestionType(q),
                    AccessType = request.AccessType,
                    IsInBank = request.IsInBank,
                    UserId = currentUserId,
                    QuestionCategoryId = q.QuestionCategoryId,
                    AttemptCount = 0
                });

                // Điền thông tin đáp án
                var now = DateTime.Now;
                for (int i = 0; i < q.Answers.Count; i++)
                {
                    answersToInsert.Add(new QuestionAnswer
                    {
                        QuestionAnswerId = Guid.NewGuid(),
                        QuestionId = questionsToInsert.Last().QuestionId,
                        StringContent = q.Answers[i].StringContent?.Trim(),
                        IsCorrectAnswer = q.Answers[i].IsCorrectAnswer,
                        OrderInList = i + 1,
                        CreatedBy = currentUserId.ToString(),
                        CreatedDate = now,
                        ModifiedBy = currentUserId.ToString(),
                        ModifiedDate = now,
                        IsDeleted = false
                    });
                }
            }

            // Ghi nhận thay đổi trong Transaction
            await _baseWriteDL.BeginTransactionAsync();
            try
            {
                await BeforeInsert(questionsToInsert);

                // Thực thi ghi DB
                if (questionsToInsert.Count > 0) await _baseWriteDL.InsertRangeAsync(questionsToInsert);
                if (answersToInsert.Count > 0) await _baseWriteDL.InsertRangeAsync(answersToInsert);
                await _baseWriteDL.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _baseWriteDL.RollbackTransactionAsync();
                throw;
            }
        }
    }
}


