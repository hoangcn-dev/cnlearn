using System;
using System.ComponentModel;

namespace HoangCN.LearnMS.Entities
{
    /// <summary>
    /// Các phương án trả lời cho câu hỏi trắc nghiệm (được lưu trữ trực tiếp dưới dạng JSON)
    /// </summary>
    public class QuestionAnswer
    {
        /// <summary>
        /// Định danh đáp án
        /// </summary>
        [DisplayName("Mã đáp án")]
        public Guid QuestionAnswerId { get; set; }

        /// <summary>
        /// Nội dung của đáp án dạng text
        /// </summary>
        [DisplayName("Nội dung đáp án")]
        public string? StringContent { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp của đáp án trong câu hỏi
        /// </summary>
        [DisplayName("Thứ tự hiển thị")]
        public int OrderInList { get; set; } = 0;

        /// <summary>
        /// Cho biết đây có phải đáp án đúng hay không (dùng để map dữ liệu đầu vào)
        /// </summary>
        [DisplayName("Độ chính xác")]
        public bool IsCorrectAnswer { get; set; } = false;
    }
}
