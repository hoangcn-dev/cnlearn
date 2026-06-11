using HoangCN.Core.Common.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoangCN.LearnMS.Entities
{
    /// <summary>
    /// Thực thể lưu thông tin đánh dấu/lưu kỳ thi của người dùng
    /// </summary>
    [Table("UserSavedQuiz")]
    public class UserSavedQuiz : BaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid UserSavedQuizId { get; set; }

        /// <summary>
        /// Mã định danh người dùng (Khóa ngoại)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Mã người dùng")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Mã định danh kỳ thi (Khóa ngoại)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Mã kỳ thi")]
        public Guid QuizId { get; set; }
    }
}
