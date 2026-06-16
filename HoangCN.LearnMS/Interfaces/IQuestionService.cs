using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
using HoangCN.LearnMS.DTOs;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HoangCN.LearnMS.Interfaces
{
    /// <summary>
    /// Giao diện nghiệp vụ câu hỏi trắc nghiệm
    /// </summary>
    public interface IQuestionService : IBaseBL<Question>
    {


        /// <summary>
        /// Lấy danh sách câu hỏi phân trang kèm chi tiết đáp án và danh mục
        /// </summary>
        Task<ResultDto<QuestionDetailsDto>> GetQuestionDetailsPagingAsync(GetRequest request, Guid currentUserId);

        /// <summary>
        /// Lấy chi tiết câu hỏi theo ID
        /// </summary>
        Task<QuestionDetailsDto?> GetQuestionDetailsByIdAsync(Guid id, Guid currentUserId);

        /// <summary>
        /// Lấy danh sách câu hỏi thuộc một đề thi
        /// </summary>
        Task<List<QuestionDetailsDto>> GetQuestionsByExamIdAsync(Guid examId, Guid currentUserId);

        /// <summary>
        /// Lưu danh sách câu hỏi chi tiết (Thêm mới/Cập nhật) kèm đáp án và danh mục
        /// </summary>
        Task SaveQuestionDetailsAsync(List<QuestionDetailsDto> questionsDto, Guid currentUserId);

        /// <summary>
        /// Lưu danh sách câu hỏi
        /// </summary>
        Task SaveListQuestions(SaveQuestionsRequest request, Guid currentUserId);

        /// <summary>
        /// Chấm điểm và trả về kết quả đáp án cho một câu hỏi
        /// </summary>
        Task<QuestionCheckResultDto> CheckAnswerAsync(QuestionCheckDto dto);
    }
}


