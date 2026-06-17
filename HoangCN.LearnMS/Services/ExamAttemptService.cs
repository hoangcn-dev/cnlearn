using HoangCN.Core.BL.Base;
using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.DL.Interfaces;
using HoangCN.LearnMS.DTOs.Attempts;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace HoangCN.LearnMS.Services
{
    public class ExamAttemptService : BaseBL<ExamAttempt>, IExamAttemptService
    {
        public ExamAttemptService(IBaseReadDL baseReadDL, IBaseWriteDL baseWriteDL, IHttpContextAccessor httpContextAccessor) 
            : base(baseReadDL, baseWriteDL, httpContextAccessor)
        {
        }

        public async Task<ExamAttemptResultDto> SubmitAttemptAsync(Guid userId, ExamAttemptSubmitDto attemptDto)
        {
            if (attemptDto == null)
                throw new BadRequestException("Dữ liệu nộp bài không hợp lệ.");

            if (attemptDto.Answers == null)
                attemptDto.Answers = new List<QuestionAnswerSubmitDto>();

            // Lấy tất cả QuestionId được gửi lên
            var questionIds = attemptDto.Answers.Select(a => a.QuestionId).Distinct().ToList();

            var dbAnswers = new List<QuestionAnswer>();
            var dbQuestions = new List<Question>();

            if (questionIds.Count > 0)
            {
                var ansSql = HoangCN.LearnMS.Utils.QuestionSqlUtil.BuildQueryAnswersByQuestionIds(questionIds, out var ansParams);
                dbAnswers = (await _baseReadDL.ExecuteQueryText<QuestionAnswer>(ansSql, ansParams)).ToList();

                var qSql = HoangCN.LearnMS.Utils.QuestionSqlUtil.BuildQueryQuestionsByIds(questionIds, out var qParams);
                dbQuestions = (await _baseReadDL.ExecuteQueryText<Question>(qSql, qParams)).ToList();
            }

            var result = new ExamAttemptResultDto
            {
                ExamAttemptId = Guid.NewGuid(),
                Duration = attemptDto.TimeSpentSeconds,
                TotalQuestions = attemptDto.Answers.Count
            };

            var attemptDetailsToInsert = new List<ExamAttemptDetail>();
            int correctCount = 0;

            foreach (var userAns in attemptDto.Answers)
            {
                var qDbAnswers = dbAnswers.Where(a => a.QuestionId == userAns.QuestionId).ToList();
                var correctDbAnswerIds = qDbAnswers.Where(a => a.IsCorrectAnswer).Select(a => a.QuestionAnswerId).ToList();

                var isCorrect = false;

                // So sánh logic:
                // Nếu câu hỏi có nhiều đáp án đúng: User phải chọn đủ và đúng tất cả.
                // Nếu 1 đáp án đúng: User chọn đúng 1 ID đó.
                if (correctDbAnswerIds.Any())
                {
                    var userSelectedIds = userAns.SelectedAnswerIds ?? new List<Guid>();
                    if (correctDbAnswerIds.Count == userSelectedIds.Count &&
                        correctDbAnswerIds.All(id => userSelectedIds.Contains(id)))
                    {
                        isCorrect = true;
                    }
                }

                if (isCorrect)
                {
                    correctCount++;
                }

                var user = _httpContextAccessor.HttpContext?.User;
                var currentUserName = (user != null && user.Identity?.IsAuthenticated == true)
                    ? HoangCN.Core.Common.Utils.ClaimUtil.GetUserName(user)
                    : userId.ToString();
                var now = DateTime.UtcNow;

                var detail = new ExamAttemptDetail
                {
                    ExamAttemptDetailId = Guid.NewGuid(),
                    ExamAttemptId = result.ExamAttemptId,
                    QuestionId = userAns.QuestionId,
                    SelectedAnswerIds = userAns.SelectedAnswerIds != null ? string.Join(",", userAns.SelectedAnswerIds) : "",
                    IsCorrect = isCorrect,
                    CreatedBy = currentUserName,
                    CreatedDate = now
                };

                attemptDetailsToInsert.Add(detail);

                var qEntity = dbQuestions.FirstOrDefault(q => q.QuestionId == userAns.QuestionId);

                result.Details.Add(new QuestionAttemptResultDto
                {
                    QuestionId = userAns.QuestionId,
                    IsCorrect = isCorrect,
                    SelectedAnswerIds = userAns.SelectedAnswerIds ?? new List<Guid>(),
                    CorrectAnswerIds = correctDbAnswerIds,
                    Explanation = qEntity?.Explaination ?? ""
                });
            }

            result.CorrectCount = correctCount;
            result.Score = result.TotalQuestions > 0 ? (decimal)correctCount / result.TotalQuestions * 10 : 0;

            var userAuth = _httpContextAccessor.HttpContext?.User;
            var userName = (userAuth != null && userAuth.Identity?.IsAuthenticated == true)
                ? HoangCN.Core.Common.Utils.ClaimUtil.GetUserName(userAuth)
                : userId.ToString();
            var finishedDate = DateTime.UtcNow;

            var examAttempt = new ExamAttempt
            {
                ExamAttemptId = result.ExamAttemptId,
                UserId = userId,
                ExamId = attemptDto.ExamId,
                QuizId = attemptDto.QuizId,
                AttemptType = attemptDto.AttemptType,
                Score = result.Score,
                TotalQuestions = result.TotalQuestions,
                CorrectCount = result.CorrectCount,
                Duration = result.Duration,
                StartedDate = finishedDate.AddSeconds(-result.Duration),
                FinishedDate = finishedDate,
                CreatedBy = userName,
                CreatedDate = finishedDate
            };

            await _baseWriteDL.BeginTransactionAsync();
            try
            {
                await _baseWriteDL.InsertRangeAsync(new List<ExamAttempt> { examAttempt });
                if (attemptDetailsToInsert.Count > 0)
                {
                    await _baseWriteDL.InsertRangeAsync(attemptDetailsToInsert);
                }
                await _baseWriteDL.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _baseWriteDL.RollbackTransactionAsync();
                throw new BadRequestException("Lỗi lưu lịch sử làm bài: " + ex.Message);
            }

            return result;
        }

        public async Task<ResultDto<AttemptHistoryDto>> GetExamAttemptHistoryAsync(Guid userId, int pageIndex, int pageSize)
        {
            var offset = (pageIndex - 1) * pageSize;
            var sqlCount = HoangCN.LearnMS.Utils.ExamAttemptSqlUtil.BuildQueryExamHistoryCount(userId, out var countParams);
            var totalItems = await _baseReadDL.ExecuteQueryToGetFirstResult<int>(sqlCount, countParams);

            var sqlData = HoangCN.LearnMS.Utils.ExamAttemptSqlUtil.BuildQueryExamHistoryPaging(userId, pageSize, offset, out var dataParams);
            var items = await _baseReadDL.ExecuteQueryText<AttemptHistoryDto>(sqlData, dataParams);

            return new ResultDto<AttemptHistoryDto>
            {
                Page = pageIndex,
                Size = pageSize,
                Total = totalItems,
                Items = items?.ToList() ?? new List<AttemptHistoryDto>()
            };
        }

        public async Task<ResultDto<AttemptHistoryDto>> GetQuestionAttemptHistoryAsync(Guid userId, int pageIndex, int pageSize)
        {
            var offset = (pageIndex - 1) * pageSize;
            var sqlCount = HoangCN.LearnMS.Utils.ExamAttemptSqlUtil.BuildQueryQuestionHistoryCount(userId, out var countParams);
            var totalItems = await _baseReadDL.ExecuteQueryToGetFirstResult<int>(sqlCount, countParams);

            var sqlData = HoangCN.LearnMS.Utils.ExamAttemptSqlUtil.BuildQueryQuestionHistoryPaging(userId, pageSize, offset, out var dataParams);
            var items = await _baseReadDL.ExecuteQueryText<AttemptHistoryDto>(sqlData, dataParams);

            return new ResultDto<AttemptHistoryDto>
            {
                Page = pageIndex,
                Size = pageSize,
                Total = totalItems,
                Items = items?.ToList() ?? new List<AttemptHistoryDto>()
            };
        }
    }
}
