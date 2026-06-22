using HoangCN.Core.BL.Interfaces;
using HoangCN.LearnMS.DTOs;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Requests;

namespace HoangCN.LearnMS.Interfaces
{
    /// <summary>
    /// Giao diện nghiệp vụ câu hỏi trắc nghiệm
    /// </summary>
    public interface IQuestionService : IBaseBL<Question>
    {
        /// <summary>
        /// Lấy mapping đáp án đúng theo câu hỏi
        /// </summary>
        Task<QuestionCorrectAnswerMappingDto> GetQuestionCorrectAnswer(Guid? userId, List<Guid> questionIds);

        /// <summary>
        /// Lấy về thông tin của 1 câu hỏi (ko bao gồm câu trả lời)
        /// </summary>
        Task<List<QuestionAnswerDto>> GetAnswersContent(Guid? userId, List<Guid> questionIds);

        /// <summary>
        /// Lấy về thông tin đáp án của danh sách câu hỏi (ko bao gồm câu trả lời)
        /// </summary>
        Task<QuestionDto> GetQuestionContent(Guid? userId, Guid questionId);

        /// <summary>
        /// Lấy danh sách các câu hỏi đã lưu (yêu thích) của một người dùng cụ thể
        /// </summary>
        /// <param name="userId">Mã định danh của người dùng</param>
        /// <returns>Danh sách DTO chứa thông tin các câu hỏi đã được lưu</returns>
        Task<List<SavedDto>> GetSavedQuestions(Guid userId);

        /// <summary>
        /// Thực hiện bật/tắt (lưu hoặc bỏ lưu) một câu hỏi vào danh sách yêu thích của người dùng
        /// </summary>
        /// <param name="request">Yêu cầu chứa thông tin UserId, TargetId (Id câu hỏi) và trạng thái lưu</param>
        /// <returns>Tác vụ xử lý bất đồng bộ</returns>
        Task ToggleQuestion(ToggleUserSavedRequest request);
    }
}


