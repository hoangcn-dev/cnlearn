using HoangCN.Core.BL.Interfaces;
using HoangCN.LearnMS.DTOs;
using HoangCN.LearnMS.Entities;

namespace HoangCN.LearnMS.Interfaces
{
    /// <summary>
    /// Giao diện nghiệp vụ đề thi
    /// </summary>
    public interface IExamService : IBaseBL<Exam>
    {
        /// <summary>
        /// Lấy chi tiết đề thi có kiểm tra quyền truy cập riêng tư
        /// </summary>
        Task<ExamDto> GetExamByIdAsync(Guid? userId, Guid examId);

        /// <summary>
        /// Lấy danh sách câu hỏi thuộc đề thi và xếp đúng thứ tự
        /// </summary>
        Task<List<ExamQuestionDto>> GetExamQuestionsAsync(Guid? userId, Guid examId);

        Task<QuestionCorrectAnswerMappingDto> GetExamCorrectKeys(Guid? userId, Guid examId);

        Task<List<QuestionAnswerDto>> GetAnswersContent(Guid? userId, Guid examId);
    }
}
