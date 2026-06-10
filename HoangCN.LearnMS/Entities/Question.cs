using HoangCN.Core.Common.Base;
using HoangCN.Core.Common.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.LearnMS.Entities
{
    /// <summary>
    /// Bảng quản lý thông tin câu hỏi trắc nghiệm
    /// </summary>
    public class Question : BaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Chuỗi ngắn dùng cho SEO
        /// </summary>
        [DisplayName("Đường dẫn thân thiện (Slug)")]
        [StringLength(255, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        public string? Slug { get; set; }

        /// <summary>
        /// Nội dung câu hỏi bằng chữ
        /// </summary>
        [DisplayName("Nội dung câu hỏi")]
        public string? StringContent { get; set; }

        /// <summary>
        /// Giải thích chi tiết đáp án câu hỏi
        /// </summary>
        [DisplayName("Giải thích")]
        public string? Explaination { get; set; }

        /// <summary>
        /// Số lượt học sinh đã làm câu hỏi này
        /// </summary>
        [DisplayName("Số lượt làm")]
        public int AttemptCount { get; set; } = 0;

        /// <summary>
        /// Mức độ khó của câu hỏi
        /// </summary>
        [DisplayName("Mức độ")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public QuestionLevel Level { get; set; } = QuestionLevel.Easy;

        /// <summary>
        /// Loại câu hỏi (Một lựa chọn hay Nhiều lựa chọn)
        /// </summary>
        [DisplayName("Loại câu hỏi")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public QuestionType Type { get; set; } = QuestionType.SingleChoice;

        /// <summary>
        /// Định danh tài khoản người sở hữu câu hỏi
        /// </summary>
        [DisplayName("Tài khoản sở hữu")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Quyền truy cập của câu hỏi
        /// </summary>
        [DisplayName("Quyền truy cập")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public QuestionAccessType AccessType { get; set; } = QuestionAccessType.Public;
    }
}
