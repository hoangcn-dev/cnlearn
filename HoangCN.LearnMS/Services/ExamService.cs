using HoangCN.Core.BL.Base;
using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.DL.Interfaces;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;
using HoangCN.LearnMS.DTOs;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace HoangCN.LearnMS.Services
{
    /// <summary>
    /// Triển khai nghiệp vụ quản lý đề thi
    /// </summary>
    public class ExamService : BaseBL<Exam>, IExamService
    {
        private readonly IQuestionService _questionService;
        private readonly IBaseBL<ExamQuestion> _examQuestionBL;

        public ExamService(
            IBaseReadDL baseReadDL, 
            IBaseWriteDL baseWriteDL,
            IQuestionService questionService,
            IBaseBL<ExamQuestion> examQuestionBL,
            IHttpContextAccessor httpContextAccessor) : base(baseReadDL, baseWriteDL, httpContextAccessor)
        {
            _questionService = questionService ?? throw new ArgumentNullException(nameof(questionService));
            _examQuestionBL = examQuestionBL ?? throw new ArgumentNullException(nameof(examQuestionBL));
        }

        /// <summary>
        /// Lấy danh sách đề thi kèm theo việc lọc riêng biệt các đề private/nháp
        /// </summary>
        public async Task<ResultDto<Exam>> GetExamsPagingAsync(GetRequest request, Guid? currentUserId)
        {
            Expression<Func<Exam, bool>>? condition = null;
            if (currentUserId.HasValue)
            {
                condition = e => e.AccessType == 1 || e.UserId == currentUserId.Value;
            }
            else
            {
                condition = e => e.AccessType == 1;
            }
            var result = await base.Get<Exam>(request, condition);
            
            if (currentUserId.HasValue && result != null && result.Items != null)
            {
                foreach (var exam in result.Items)
                {
                    exam.IsMyCreated = exam.UserId == currentUserId.Value;
                }
            }
            return result!;
        }
        /// <summary>
        /// Lưu chi tiết đề thi và danh sách câu hỏi đi kèm trong một Transaction
        /// </summary>
        public async Task<Guid> SaveExamDetailsAsync(ExamSaveDto dto, Guid currentUserId)
        {
            if (dto == null)
            {
                throw new BadRequestException("Dữ liệu trống hoặc sai định dạng");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new BadRequestException("Tên đề thi không được phép để trống.");
            }

            var isNew = !dto.ExamId.HasValue || dto.ExamId.Value == Guid.Empty;
            var examId = isNew ? Guid.NewGuid() : dto.ExamId!.Value;

            await _baseWriteDL.BeginTransactionAsync();
            try
            {
                var queryableExam = _baseWriteDL.GetQueryable<Exam>();

                // 1. Lưu thông tin đề thi
                Exam exam;
                if (isNew)
                {
                    exam = new Exam
                    {
                        ExamId = examId,
                        UserId = currentUserId
                    };
                }
                else
                {
                    var existing = await queryableExam.FirstOrDefaultAsync(e => e.ExamId == examId && !e.IsDeleted);
                    if (existing == null)
                    {
                        exam = new Exam
                        {
                            ExamId = examId,
                            UserId = currentUserId
                        };
                        isNew = true;
                    }
                    else
                    {
                        exam = existing;
                        isNew = false;
                    }
                }

                exam.Name = dto.Name.Trim();
                exam.Description = dto.Description?.Trim();
                exam.CategoryId = dto.CategoryId;
                exam.Duration = dto.Duration;
                exam.AccessType = dto.AccessType;
                exam.IsDraft = dto.IsDraft;
                exam.ContributeToBank = dto.ContributeToBank;

                var now = DateTime.Now;
                if (isNew)
                {
                    exam.CreatedBy = currentUserId.ToString();
                    exam.CreatedDate = now;
                    exam.ModifiedBy = currentUserId.ToString();
                    exam.ModifiedDate = now;
                }
                else
                {
                    exam.ModifiedBy = currentUserId.ToString();
                    exam.ModifiedDate = now;
                }

                if (isNew)
                {
                    await _baseWriteDL.InsertRangeAsync(new List<Exam> { exam });
                }
                else
                {
                    await _baseWriteDL.UpdateRangeAsync(new List<Exam> { exam });
                }

                // 2. Lưu thông tin các câu hỏi trắc nghiệm liên quan
                var questionsToSave = new List<QuestionDetailsDto>();

                for (int i = 0; i < dto.Questions.Count; i++)
                {
                    var qDto = dto.Questions[i];
                    var isGuid = Guid.TryParse(qDto.Id.ToString(), out Guid questionGuid);
                    if (!isGuid || questionGuid == Guid.Empty)
                    {
                        questionGuid = Guid.NewGuid();
                    }

                    qDto.Id = questionGuid;
                    questionsToSave.Add(qDto);
                }

                if (questionsToSave.Count > 0)
                {
                    await _questionService.SaveQuestionDetailsAsync(questionsToSave, currentUserId);
                }

                // 3. Xoá các mối liên kết đề thi - câu hỏi cũ
                var existingRelations = await _baseWriteDL.GetQueryable<ExamQuestion>()
                    .Where(eq => eq.ExamId == examId && !eq.IsDeleted)
                    .ToListAsync();

                foreach (var relation in existingRelations)
                {
                    relation.IsDeleted = true;
                }

                if (existingRelations.Count > 0)
                {
                    await _baseWriteDL.DeleteRangeAsync(existingRelations);
                }

                // 4. Tạo các mối liên kết đề thi - câu hỏi mới
                var newRelations = new List<ExamQuestion>();
                for (int i = 0; i < dto.Questions.Count; i++)
                {
                    var qDto = dto.Questions[i];
                    var relation = new ExamQuestion
                    {
                        ExamQuestionId = Guid.NewGuid(),
                        ExamId = examId,
                        QuestionId = qDto.Id,
                        SortOrder = i + 1,
                        CreatedBy = currentUserId.ToString(),
                        CreatedDate = now,
                        ModifiedBy = currentUserId.ToString(),
                        ModifiedDate = now
                    };
                    newRelations.Add(relation);
                }

                if (newRelations.Count > 0)
                {
                    await _baseWriteDL.InsertRangeAsync(newRelations);
                }

                await _baseWriteDL.CommitTransactionAsync();
                return examId;
            }
            catch (Exception)
            {
                await _baseWriteDL.RollbackTransactionAsync();
                throw;
            }
        }

        private class ExamQuestionCountQueryResult
        {
            public Guid ExamId { get; set; }
            public int Count { get; set; }
        }

        /// <summary>
        /// Lấy số lượng câu hỏi của tất cả đề thi sử dụng Dapper SQL GROUP BY trực tiếp ở DB
        /// </summary>
        public async Task<Dictionary<Guid, int>> GetQuestionCountsAsync()
        {
            var sql = "SELECT ExamId, COUNT(*) as Count FROM ExamQuestion WHERE IsDeleted = 0 GROUP BY ExamId";
            var queryResult = await _baseReadDL.ExecuteQueryText<ExamQuestionCountQueryResult>(sql);
            return queryResult.ToDictionary(x => x.ExamId, x => x.Count);
        }
    }
}
