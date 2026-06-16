using HoangCN.Core.Common.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoangCN.LearnMS.Entities
{
    /// <summary>
    /// Bảng trung gian liên kết câu hỏi vào đề thi và lưu thứ tự sắp xếp
    /// </summary>
    [Table("ExamQuestion")]
    public class ExamQuestion : BaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        [DisplayName("Mã liên kết đề - câu hỏi")]
        public Guid ExamQuestionId { get; set; }

        /// <summary>
        /// Mã định danh đề thi (Khóa ngoại)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Mã đề thi")]
        public Guid ExamId { get; set; }

        /// <summary>
        /// Mã định danh câu hỏi (Khóa ngoại)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Mã câu hỏi")]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Thứ tự câu hỏi trong đề
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Thứ tự hiển thị")]
        public int SortOrder { get; set; } = 0;
    }
}
