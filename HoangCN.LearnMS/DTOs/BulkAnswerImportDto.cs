using System;

namespace HoangCN.LearnMS.DTOs
{
    /// <summary>
    /// Đối tượng DTO đại diện cho một câu trả lời trong quá trình import câu hỏi hàng loạt
    /// </summary>
    public class BulkAnswerImportDto
    {
        /// <summary>
        /// Nội dung câu trả lời dạng chữ
        /// </summary>
        public string? StringContent { get; set; }

        /// <summary>
        /// Đánh dấu xem đây có phải đáp án đúng hay không
        /// </summary>
        public bool IsCorrectAnswer { get; set; }
    }
}

