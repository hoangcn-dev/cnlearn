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
using System.Text.Json;
using System.Threading.Tasks;

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
            ILogger<QuestionService> logger) : base(baseReadDL, baseWriteDL)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task BeforeSave(List<Question> entities)
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

            await base.BeforeSave(entities);
        }

        /// <summary>
        /// Thực hiện phân tích chuỗi JSON và tạo danh sách câu hỏi kèm đáp án, gán nhiều danh mục theo ID trong một Transaction duy nhất
        /// </summary>
        public async Task<int> ImportBulkFromJsonAsync(string jsonContent, Guid currentUserId)
        {
            // Kiểm tra quy tắc Rule 1: Throw exception nếu chuỗi null hoặc rỗng
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new BadRequestException("Nội dung file JSON import không được phép để trống.");
            }

            // Kiểm tra UserId người sở hữu không được rỗng
            if (currentUserId == Guid.Empty)
            {
                throw new BadRequestException("Mã định danh tài khoản sở hữu không hợp lệ.");
            }

            List<BulkQuestionImportDto> questionsDto;
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                };
                questionsDto = JsonSerializer.Deserialize<List<BulkQuestionImportDto>>(jsonContent, options)
                               ?? throw new BadRequestException("Định dạng file JSON không hợp lệ.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi phân tích cú pháp JSON câu hỏi.");
                throw new BadRequestException($"Lỗi cú pháp file JSON: {ex.Message}");
            }

            if (questionsDto.Count == 0)
            {
                throw new BadRequestException("Không tìm thấy danh sách câu hỏi nào trong file JSON.");
            }

            var questionsToInsert = new List<Question>();
            var relationsToInsert = new List<QuestionInCategory>();
            var answersToInsert = new List<QuestionAnswer>();

            // Bắt đầu một Transaction duy nhất thông qua _baseWriteDL
            await _baseWriteDL.BeginTransactionAsync();
            try
            {
                foreach (var qDto in questionsDto)
                {
                    if (string.IsNullOrWhiteSpace(qDto.StringContent))
                    {
                        throw new BadRequestException("Nội dung câu hỏi không được phép để trống.");
                    }
                    if (qDto.CategoryIds == null || qDto.CategoryIds.Count == 0)
                    {
                        throw new BadRequestException($"Câu hỏi '{qDto.StringContent}' phải được gán ít nhất một ID danh mục (CategoryIds).");
                    }
                    if (qDto.Answers == null || qDto.Answers.Count == 0)
                    {
                        throw new BadRequestException($"Câu hỏi '{qDto.StringContent}' phải có ít nhất một đáp án lựa chọn.");
                    }

                    // 1. Kiểm tra toàn bộ danh mục phải tồn tại trước (Rule 4) bằng _categoryService (Rule 6 - Tối đa hóa tái sử dụng)
                    foreach (var catId in qDto.CategoryIds)
                    {
                        var existingCats = await _categoryService.GetByCondition<QuestionCategory>(c => c.QuestionCategoryId == catId && !c.IsDeleted);
                        if (!existingCats.Any())
                        {
                            throw new BadRequestException($"Danh mục với ID '{catId}' chưa tồn tại trong hệ thống. Vui lòng tạo danh mục trước.");
                        }
                    }

                    // 2. Tạo Slug tự động bằng tiện ích SlugUtil trong Common
                    string slug = string.IsNullOrWhiteSpace(qDto.Slug)
                        ? SlugUtil.GenerateSlug(qDto.StringContent)
                        : SlugUtil.GenerateSlug(qDto.Slug);

                    if (string.IsNullOrWhiteSpace(slug))
                    {
                        throw new BadRequestException($"Không thể tạo đường dẫn SEO (Slug) hợp lệ cho câu hỏi: '{qDto.StringContent}'");
                    }

                    // Kiểm tra câu hỏi trùng slug bằng GetByCondition (Rule 6)
                    var existingQs = await GetByCondition<Question>(q => q.Slug == slug && !q.IsDeleted);
                    if (existingQs.Any())
                    {
                        _logger.LogWarning("Bỏ qua câu hỏi đã tồn tại với Slug: {Slug}", slug);
                        continue; // Trùng thì bỏ qua câu hỏi này
                    }

                    // Khởi tạo đối tượng câu hỏi
                    var questionId = Guid.NewGuid();
                    var question = new Question
                    {
                        QuestionId = questionId,
                        Slug = slug,
                        StringContent = qDto.StringContent.Trim(),
                        Explaination = qDto.Explaination?.Trim(),
                        AttemptCount = 0,
                        Level = (QuestionLevel)qDto.Level,
                        Type = (QuestionType)qDto.Type,
                        UserId = currentUserId, // Tài khoản sở hữu
                        AccessType = (QuestionAccessType)qDto.AccessType, // Quyền truy cập
                        CreatedBy = "System Bulk Import",
                        CreatedDate = DateTime.Now,
                        State = ModelState.Insert
                    };
                    questionsToInsert.Add(question);

                    // Khởi tạo bảng trung gian cho từng danh mục được liên kết (Truyền nhiều danh mục)
                    for (int i = 0; i < qDto.CategoryIds.Count; i++)
                    {
                        var catId = qDto.CategoryIds[i];
                        var relation = new QuestionInCategory
                        {
                            QuestionId = questionId,
                            QuestionCategoryId = catId,
                            OrderInList = i + 1, // Thứ tự liên kết bắt đầu từ 1 dựa trên thứ tự xuất hiện trong danh sách CategoryIds
                            CreatedBy = "System Bulk Import",
                            CreatedDate = DateTime.Now,
                            State = ModelState.Insert
                        };
                        relationsToInsert.Add(relation);
                    }

                    // 3. Duyệt đáp án
                    var hasCorrectAnswer = false;
                    for (int j = 0; j < qDto.Answers.Count; j++)
                    {
                        var aDto = qDto.Answers[j];
                        if (string.IsNullOrWhiteSpace(aDto.StringContent))
                        {
                            throw new BadRequestException($"Nội dung đáp án cho câu hỏi '{qDto.StringContent}' không được phép để trống.");
                        }

                        var answer = new QuestionAnswer
                        {
                            QuestionAnswerId = Guid.NewGuid(),
                            QuestionId = questionId,
                            StringContent = aDto.StringContent.Trim(),
                            IsCorrectAnswer = aDto.IsCorrectAnswer,
                            OrderInList = j + 1, // Thứ tự của câu trả lời tự động gán từ 1 dựa trên vị trí mảng
                            CreatedBy = "System Bulk Import",
                            CreatedDate = DateTime.Now,
                            State = ModelState.Insert
                        };
                        answersToInsert.Add(answer);

                        if (aDto.IsCorrectAnswer)
                        {
                            hasCorrectAnswer = true;
                        }
                    }

                    if (!hasCorrectAnswer)
                    {
                        throw new BadRequestException($"Câu hỏi '{qDto.StringContent}' không có phương án nào được đánh dấu là đúng.");
                    }
                }

                // 4. Lưu tất cả các danh sách trong transaction
                if (questionsToInsert.Count > 0)
                {
                    await _baseWriteDL.SaveEntitiesAsync(questionsToInsert);
                }

                if (relationsToInsert.Count > 0)
                {
                    await _baseWriteDL.SaveEntitiesAsync(relationsToInsert);
                }

                if (answersToInsert.Count > 0)
                {
                    await _baseWriteDL.SaveEntitiesAsync(answersToInsert);
                }

                await _baseWriteDL.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _baseWriteDL.RollbackTransactionAsync();
                _logger.LogError(ex, "Thất bại khi thực thi ImportBulkFromJson. Đã rollback.");
                throw;
            }

            return questionsToInsert.Count;
        }

        /// <summary>
        /// Tiền xử lý xóa - thực hiện xóa mềm/cascade các thực thể liên quan
        /// </summary>
        protected override void BeforeDelete(List<Question> entities)
        {
            var dbContext = ((HoangCN.Core.DL.Implementation.BaseWriteDL)_baseWriteDL).Context;
            var questionIds = entities.Select(q => q.QuestionId).ToList();

            // 1. Xóa mềm đáp án
            var answers = dbContext.Set<QuestionAnswer>()
                .Where(a => questionIds.Contains(a.QuestionId) && !a.IsDeleted)
                .ToList();
            foreach (var a in answers)
            {
                a.IsDeleted = true;
                a.State = ModelState.Delete;
                dbContext.Entry(a).State = EntityState.Modified;
            }

            // 2. Xóa mềm liên kết danh mục
            var relations = dbContext.Set<QuestionInCategory>()
                .Where(r => questionIds.Contains(r.QuestionId) && !r.IsDeleted)
                .ToList();
            foreach (var r in relations)
            {
                r.IsDeleted = true;
                r.State = ModelState.Delete;
                dbContext.Entry(r).State = EntityState.Modified;
            }

            base.BeforeDelete(entities);
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

            // Xử lý bộ lọc UserId "mine"
            var userIdFilter = request.Filters.FirstOrDefault(f => f != null && string.Equals(f.Property, "UserId", StringComparison.OrdinalIgnoreCase));
            if (userIdFilter != null && userIdFilter.Value?.ToString() == "mine")
            {
                userIdFilter.Value = currentUserId.ToString();
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

            // Xử lý bộ lọc CategoryId
            var categoryFilter = request.Filters.FirstOrDefault(f => f != null && string.Equals(f.Property, "CategoryId", StringComparison.OrdinalIgnoreCase));
            if (categoryFilter != null)
            {
                request.Filters.Remove(categoryFilter);

                if (Guid.TryParse(categoryFilter.Value?.ToString(), out Guid catId))
                {
                    var sql = QuestionSqlUtil.BuildQueryQuestionIdsByCategory(catId, out var catParams);
                    var relationsForCategory = await _baseReadDL.ExecuteQueryText<QuestionInCategory>(sql, catParams);
                    var questionIdsForCategory = (relationsForCategory ?? Enumerable.Empty<QuestionInCategory>())
                        .Where(r => r != null)
                        .Select(r => r.QuestionId)
                        .ToList();

                    if (questionIdsForCategory.Count == 0)
                    {
                        return result; // Trả về danh sách rỗng nếu không có câu hỏi nào trong danh mục này
                    }

                    if (request.Ids.Count > 0)
                    {
                        request.Ids = request.Ids.Intersect(questionIdsForCategory).ToList();
                    }
                    else
                    {
                        request.Ids = questionIdsForCategory;
                    }

                    if (request.Ids.Count == 0)
                    {
                        return result; // Trả về danh sách rỗng sau khi giao nhau
                    }
                }
            }

            var questionsResult = await Get<Question>(request);
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

            // Lấy danh sách liên kết danh mục qua raw SQL tập trung
            var relSql = QuestionSqlUtil.BuildQueryCategoriesByQuestionIds(questionIds, out var relParams);
            var relations = await _baseReadDL.ExecuteQueryText<QuestionInCategory>(relSql, relParams);

            var answerGroup = (answers ?? Enumerable.Empty<QuestionAnswer>())
                .Where(a => a != null)
                .GroupBy(a => a.QuestionId)
                .ToDictionary(g => g.Key, g => g.OrderBy(a => a.OrderInList).ToList());

            var relationGroup = (relations ?? Enumerable.Empty<QuestionInCategory>())
                .Where(r => r != null)
                .GroupBy(r => r.QuestionId)
                .ToDictionary(g => g.Key, g => g.OrderBy(r => r.OrderInList).Select(r => r.QuestionCategoryId).ToList());

            foreach (var q in questionsResult.Items)
            {
                if (q == null) continue;

                answerGroup.TryGetValue(q.QuestionId, out var qAnswers);
                relationGroup.TryGetValue(q.QuestionId, out var qCategoryIds);

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
                    CategoryIds = qCategoryIds ?? new List<Guid>(),
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

            // Lấy danh sách liên kết danh mục qua raw SQL tập trung
            var relSql = QuestionSqlUtil.BuildQueryCategoriesByQuestionId(id, out var relParams);
            var relations = await _baseReadDL.ExecuteQueryText<QuestionInCategory>(relSql, relParams);

            return new QuestionDetailsDto
            {
                Id = q.QuestionId,
                Slug = q.Slug,
                StringContent = q.StringContent,
                Explanation = q.Explaination,
                Level = (int)q.Level,
                Type = (int)q.Type,
                AccessType = (int)q.AccessType,
                IsMyCreated = q.UserId == currentUserId,
                CategoryIds = relations.Select(r => r.QuestionCategoryId).ToList(),
                Answers = answers.Select(a => new AnswerDetailsDto
                {
                    QuestionAnswerId = a.QuestionAnswerId,
                    StringContent = a.StringContent,
                    IsCorrectAnswer = a.IsCorrectAnswer
                }).ToList()
            };
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

            var questionsToSave = new List<Question>();
            var answersToSave = new List<QuestionAnswer>();
            var relationsToSave = new List<QuestionInCategory>();

            var dbContext = ((HoangCN.Core.DL.Implementation.BaseWriteDL)_baseWriteDL).Context;

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
                if (isNew)
                {
                    q = new Question
                    {
                        QuestionId = questionId,
                        UserId = currentUserId,
                        State = ModelState.Insert
                    };
                }
                else
                {
                    var existingList = await GetByCondition<Question>(x => x.QuestionId == questionId);
                    q = existingList.FirstOrDefault();
                    if (q == null)
                    {
                        q = new Question
                        {
                            QuestionId = questionId,
                            UserId = currentUserId,
                            State = ModelState.Insert
                        };
                    }
                    else
                    {
                        q.State = ModelState.Update;
                    }
                }

                q.Slug = slug;
                q.StringContent = qDto.StringContent?.Trim();
                q.Explaination = qDto.Explanation?.Trim();
                q.Level = (QuestionLevel)qDto.Level;
                q.Type = (QuestionType)qDto.Type;
                q.AccessType = (QuestionAccessType)qDto.AccessType;
                questionsToSave.Add(q);

                // Load answers hiện tại
                var existingAnswers = new List<QuestionAnswer>();
                if (!isNew)
                {
                    existingAnswers = dbContext.Set<QuestionAnswer>()
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
                        ea.State = ModelState.Delete;
                        answersToSave.Add(ea);
                    }
                }

                // Thêm hoặc cập nhật đáp án hiện tại
                for (int i = 0; i < qDto.Answers.Count; i++)
                {
                    var ansDto = qDto.Answers[i];
                    var isNewAns = ansDto.QuestionAnswerId == Guid.Empty;
                    QuestionAnswer ans;
                    if (isNewAns)
                    {
                        ans = new QuestionAnswer
                        {
                            QuestionAnswerId = Guid.NewGuid(),
                            QuestionId = questionId,
                            State = ModelState.Insert
                        };
                    }
                    else
                    {
                        ans = existingAnswers.FirstOrDefault(a => a.QuestionAnswerId == ansDto.QuestionAnswerId);
                        if (ans == null)
                        {
                            ans = new QuestionAnswer
                            {
                                QuestionAnswerId = ansDto.QuestionAnswerId,
                                QuestionId = questionId,
                                State = ModelState.Insert
                            };
                        }
                        else
                        {
                            ans.State = ModelState.Update;
                        }
                    }

                    ans.StringContent = ansDto.StringContent?.Trim();
                    ans.IsCorrectAnswer = ansDto.IsCorrectAnswer;
                    ans.OrderInList = i + 1;
                    answersToSave.Add(ans);
                }

                // Load danh mục hiện tại của câu hỏi
                var existingRelations = new List<QuestionInCategory>();
                if (!isNew)
                {
                    existingRelations = dbContext.Set<QuestionInCategory>()
                        .Where(r => r.QuestionId == questionId && !r.IsDeleted)
                        .ToList();
                }

                // Xóa các liên kết cũ không dùng nữa
                foreach (var er in existingRelations)
                {
                    if (!qDto.CategoryIds.Contains(er.QuestionCategoryId))
                    {
                        er.IsDeleted = true;
                        er.State = ModelState.Delete;
                        relationsToSave.Add(er);
                    }
                }

                // Thêm hoặc cập nhật liên kết mới
                for (int i = 0; i < qDto.CategoryIds.Count; i++)
                {
                    var catId = qDto.CategoryIds[i];
                    var rel = existingRelations.FirstOrDefault(r => r.QuestionCategoryId == catId);
                    if (rel == null)
                    {
                        rel = new QuestionInCategory
                        {
                            QuestionId = questionId,
                            QuestionCategoryId = catId,
                            OrderInList = i + 1,
                            State = ModelState.Insert
                        };
                        relationsToSave.Add(rel);
                    }
                    else
                    {
                        rel.State = ModelState.Update;
                        rel.OrderInList = i + 1;
                        relationsToSave.Add(rel);
                    }
                }
            }

            // Ghi nhận thay đổi trong Transaction
            await _baseWriteDL.BeginTransactionAsync();
            try
            {
                var now = DateTime.Now;

                // Điền thông tin Audit
                foreach (var q in questionsToSave)
                {
                    if (q.State == ModelState.Insert)
                    {
                        q.CreatedBy = "Hoàng Cao Nguyên";
                        q.CreatedDate = now;
                    }
                    q.ModifiedBy = "Hoàng Cao Nguyên";
                    q.ModifiedDate = now;
                }
                await _baseWriteDL.SaveEntitiesAsync(questionsToSave);

                foreach (var ans in answersToSave)
                {
                    if (ans.State == ModelState.Insert)
                    {
                        ans.CreatedBy = "Hoàng Cao Nguyên";
                        ans.CreatedDate = now;
                    }
                    ans.ModifiedBy = "Hoàng Cao Nguyên";
                    ans.ModifiedDate = now;
                }
                await _baseWriteDL.SaveEntitiesAsync(answersToSave);

                foreach (var rel in relationsToSave)
                {
                    if (rel.State == ModelState.Insert)
                    {
                        rel.CreatedBy = "Hoàng Cao Nguyên";
                        rel.CreatedDate = now;
                    }
                    rel.ModifiedBy = "Hoàng Cao Nguyên";
                    rel.ModifiedDate = now;
                }
                await _baseWriteDL.SaveEntitiesAsync(relationsToSave);

                await _baseWriteDL.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _baseWriteDL.RollbackTransactionAsync();
                _logger.LogError(ex, "Thất bại khi thực thi SaveQuestionDetails. Đã rollback.");
                throw;
            }
        }
    }
}


