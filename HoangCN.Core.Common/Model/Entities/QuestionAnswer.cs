using HoangCN.Common.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.Common.Model.Entities
{
    /// <summary>
    /// Bảng quản lý các phương án trả lời cho câu hỏi trắc nghiệm
    /// </summary>
    public class QuestionAnswer : BaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid QuestionAnswerId { get; set; }

        /// <summary>
        /// Nội dung của đáp án dạng text
        /// </summary>
        [DisplayName("Nội dung đáp án")]
        public string? StringContent { get; set; }

        /// <summary>
        /// Xác định đáp án này có phải là đáp án đúng hay không
        /// </summary>
        [DisplayName("Đáp án đúng")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public bool IsCorrectAnswer { get; set; } = false;

        /// <summary>
        /// Mã định danh câu hỏi sở hữu (Khóa ngoại)
        /// </summary>
        [DisplayName("Mã câu hỏi")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp của đáp án trong câu hỏi
        /// </summary>
        [DisplayName("Thứ tự hiển thị")]
        public int OrderInList { get; set; } = 0;
    }
}
