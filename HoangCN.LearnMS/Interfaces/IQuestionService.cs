using HoangCN.BL.Interfaces;
using HoangCN.Common.Model.Entities;
using System;
using System.Threading.Tasks;

namespace HoangCN.LearnMS.Interfaces
{
    /// <summary>
    /// Giao diện nghiệp vụ câu hỏi trắc nghiệm
    /// </summary>
    public interface IQuestionService : IBaseBL<Question>
    {
        /// <summary>
        /// Thực hiện phân tích chuỗi JSON và tạo danh sách câu hỏi kèm đáp án, danh mục trong một Transaction duy nhất (Không tạo mới danh mục)
        /// </summary>
        /// <param name="jsonContent">Chuỗi JSON câu hỏi</param>
        /// <param name="currentUserId">ID người dùng sở hữu</param>
        /// <returns>Số lượng câu hỏi import thành công</returns>
        Task<int> ImportBulkFromJsonAsync(string jsonContent, Guid currentUserId);
    }
}
