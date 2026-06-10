using HoangCN.Core.BL.Base;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.BL.Utils;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.LearnMS.Entities;
using HoangCN.Core.Common.Utils;
using HoangCN.Core.DL.Interfaces;
using HoangCN.LearnMS.Interfaces;
using HoangCN.LearnMS.DTOs;
using Microsoft.Extensions.Logging;
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
    }
}


