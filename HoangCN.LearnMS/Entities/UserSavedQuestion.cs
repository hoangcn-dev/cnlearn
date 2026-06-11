using HoangCN.Core.Common.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoangCN.LearnMS.Entities
{
    /// <summary>
    /// Thực thể lưu thông tin đánh dấu/lưu câu hỏi của người dùng
    /// </summary>
    [Table("UserSavedQuestion")]
    public class UserSavedQuestion : BaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid UserSavedQuestionId { get; set; }

        /// <summary>
        /// Mã định danh người dùng (Khóa ngoại)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Mã người dùng")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Mã định danh câu hỏi (Khóa ngoại)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Mã câu hỏi")]
        public Guid QuestionId { get; set; }
    }
}
