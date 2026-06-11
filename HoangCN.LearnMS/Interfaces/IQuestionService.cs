using HoangCN.Core.BL.Interfaces;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.DTOs;
using HoangCN.Core.Common.Model.DTOs;
using HoangCN.Core.Common.Model.Requests;
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
        /// Thực hiện phân tích chuỗi JSON và tạo danh sách câu hỏi
        /// </summary>
        /// <param name="jsonContent">Chuỗi JSON câu hỏi</param>
        /// <param name="currentUserId">ID người dùng sở hữu</param>
        /// <returns>Số lượng câu hỏi import thành công</returns>
        Task<int> ImportBulkFromJsonAsync(string jsonContent, Guid currentUserId);

        /// <summary>
        /// Lấy danh sách câu hỏi phân trang kèm chi tiết đáp án và danh mục
        /// </summary>
        Task<ResultDto<QuestionDetailsDto>> GetQuestionDetailsPagingAsync(GetRequest request, Guid currentUserId);

        /// <summary>
        /// Lấy chi tiết câu hỏi theo ID
        /// </summary>
        Task<QuestionDetailsDto?> GetQuestionDetailsByIdAsync(Guid id, Guid currentUserId);

        /// <summary>
        /// Lưu danh sách câu hỏi chi tiết (Thêm mới/Cập nhật) kèm đáp án và danh mục
        /// </summary>
        Task SaveQuestionDetailsAsync(List<QuestionDetailsDto> questionsDto, Guid currentUserId);
    }
}


