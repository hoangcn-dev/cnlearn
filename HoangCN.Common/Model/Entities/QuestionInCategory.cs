using HoangCN.Common.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.Common.Model.Entities
{
    /// <summary>
    /// Bảng trung gian liên kết Nhiều-Nhiều giữa câu hỏi (Question) và danh mục (QuestionCategory)
    /// </summary>
    [PrimaryKey(nameof(QuestionId), nameof(QuestionCategoryId))]
    public class QuestionInCategory : BaseEntity
    {
        /// <summary>
        /// Mã định danh của câu hỏi (Liên kết khóa ngoại)
        /// </summary>
        [Required]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Mã định danh của danh mục (Liên kết khóa ngoại)
        /// </summary>
        [Required]
        public Guid QuestionCategoryId { get; set; }

        /// <summary>
        /// Lưu thứ tự của câu hỏi trong danh mục
        /// </summary>
        [DisplayName("Thứ tự trong danh mục")]
        public int OrderInList { get; set; } = 0;
    }
}
