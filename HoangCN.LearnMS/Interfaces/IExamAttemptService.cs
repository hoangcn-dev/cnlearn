using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.LearnMS.DTOs.Attempts;
using HoangCN.LearnMS.Entities;
using System;
using System.Threading.Tasks;

namespace HoangCN.LearnMS.Interfaces
{
    public interface IExamAttemptService : IBaseBL<ExamAttempt>
    {
        Task<ExamAttemptResultDto> SubmitAttemptAsync(Guid userId, ExamAttemptSubmitDto attemptDto);
        Task<ResultDto<AttemptHistoryDto>> GetExamAttemptHistoryAsync(Guid userId, int pageIndex, int pageSize);
        Task<ResultDto<AttemptHistoryDto>> GetQuestionAttemptHistoryAsync(Guid userId, int pageIndex, int pageSize);
    }
}
