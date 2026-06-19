using HoangCN.Core.BL.Interfaces;
using HoangCN.LearnMS.DTOs;
using HoangCN.LearnMS.Entities;

namespace HoangCN.LearnMS.Interfaces
{
    /// <summary>
    /// Giao diện nghiệp vụ câu hỏi trắc nghiệm
    /// </summary>
    public interface IQuestionService : IBaseBL<Question>
    {
        Task<BankQuestionWithAnswersDto> GetBankQuestionWithAnswers(Guid questionId);

        ///// <summary>
        ///// Lấy danh sách câu hỏi phân trang kèm chi tiết đáp án và danh mục
        ///// </summary>
        //Task<ResultDto<BankQuestionDto>> GetQuestionDetailsPagingAsync(GetRequest request, Guid currentUserId);

        ///// <summary>
        ///// Lấy chi tiết câu hỏi theo ID
        ///// </summary>
        //Task<BankQuestionDto?> GetQuestionDetailsByIdAsync(Guid id, Guid currentUserId);

        ///// <summary>
        ///// Lấy danh sách câu hỏi thuộc một đề thi
        ///// </summary>
        //Task<List<BankQuestionDto>> GetQuestionsByExamIdAsync(Guid examId, Guid currentUserId);

        ///// <summary>
        ///// Lưu danh sách câu hỏi chi tiết (Thêm mới/Cập nhật) kèm đáp án và danh mục
        ///// </summary>
        //Task SaveQuestionDetailsAsync(List<BankQuestionDto> questionsDto, Guid currentUserId);

        ///// <summary>
        ///// Lưu danh sách câu hỏi
        ///// </summary>
        //Task SaveListQuestions(SaveQuestionsRequest request, Guid currentUserId);

        ///// <summary>
        ///// Chấm điểm và trả về kết quả đáp án cho một câu hỏi
        ///// </summary>
        //Task<QuestionCheckResultDto> CheckAnswerAsync(QuestionCheckDto dto);
    }
}


