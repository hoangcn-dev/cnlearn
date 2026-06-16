using HoangCN.Core.Common.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.LearnMS.Entities
{
    /// <summary>
    /// Bảng quản lý danh mục câu hỏi trắc nghiệm
    /// </summary>
    public class QuestionCategory : BaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid QuestionCategoryId { get; set; }

        /// <summary>
        /// Đường dẫn thân thiện cho SEO
        /// </summary>
        [DisplayName("Đường dẫn SEO (Slug)")]
        [StringLength(255, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        public string? Slug { get; set; }

        /// <summary>
        /// Tên danh mục câu hỏi
        /// </summary>
        [DisplayName("Tên danh mục")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(255, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mã danh mục cha (để phân cấp danh mục)
        /// </summary>
        [DisplayName("Danh mục cha")]
        public Guid? ParentId { get; set; }
    }
}
