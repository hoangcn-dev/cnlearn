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
        /// 
        /// </summary>
        Task<BankQuestionWithAnswersDto> GetBankQuestionWithAnswers(Guid questionId);

        Task<List<SavedDto>> GetSavedQuestions(Guid userId);

        Task ToggleQuestion(ToggleUserSavedRequest request);
    }
}


