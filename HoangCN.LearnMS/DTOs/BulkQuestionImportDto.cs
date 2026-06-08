using System;
using System.Collections.Generic;

namespace HoangCN.LearnMS.DTOs
{
    /// <summary>
    /// Đối tượng DTO đại diện cho một câu hỏi trong quá trình import câu hỏi hàng loạt từ JSON
    /// </summary>
    public class BulkQuestionImportDto
    {
        /// <summary>
        /// Đường dẫn SEO thân thiện
        /// </summary>
        public string? Slug { get; set; }

        /// <summary>
        /// Nội dung câu hỏi dạng chữ
        /// </summary>
        public string? StringContent { get; set; }

        /// <summary>
        /// Giải thích chi tiết đáp án câu hỏi
        /// </summary>
        public string? Explaination { get; set; }

        /// <summary>
        /// Mức độ khó của câu hỏi (0 = Easy, 1 = Medium, 2 = Hard)
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Loại câu hỏi (0 = SingleChoice, 1 = MultipleChoice)
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Quyền truy cập của câu hỏi (0 = Public, 1 = Private)
        /// </summary>
        public int AccessType { get; set; }

        /// <summary>
        /// Danh sách ID của các danh mục liên kết với câu hỏi
        /// </summary>
        public List<Guid> CategoryIds { get; set; } = new();

        /// <summary>
        /// Danh sách các câu trả lời tương ứng
        /// </summary>
        public List<BulkAnswerImportDto> Answers { get; set; } = new();
    }
}
