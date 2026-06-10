using HoangCN.Core.Common.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.LearnMS.Entities
{
    /// <summary>
    /// Bảng lưu trữ liên kết ảnh, video, hoặc âm thanh cho đáp án câu hỏi trắc nghiệm
    /// </summary>
    public class QuestionAnswerMedia : BaseEntity
    {
        /// <summary>
        /// Khóa chính (Mã file tài nguyên tương ứng, trùng khớp với ResourceFileId)
        /// </summary>
        [Key]
        public Guid FileId { get; set; }

        /// <summary>
        /// Mã phương án trả lời sở hữu media này
        /// </summary>
        [DisplayName("Mã đáp án")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public Guid QuestionAnswerId { get; set; }

        /// <summary>
        /// Nhãn hiển thị của file phương tiện (Ví dụ: Minh họa đáp án A)
        /// </summary>
        [DisplayName("Nhãn hiển thị")]
        [StringLength(255, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        public string? Label { get; set; }

        /// <summary>
        /// Thứ tự hiển thị phương tiện của đáp án
        /// </summary>
        [DisplayName("Thứ tự hiển thị")]
        public int OrderInList { get; set; } = 0;
    }
}
