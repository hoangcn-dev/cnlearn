using HoangCN.Core.Common.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.MainSystem.Entities
{
    /// <summary>
    /// Bảng lưu trữ mẫu Email (Email Template) trong cơ sở dữ liệu
    /// </summary>
    [Index(nameof(TemplateCode), IsUnique = true)]
    public class EmailTemplate : BaseEntity
    {
        /// <summary>
        /// Định danh duy nhất cho tệp tài nguyên template dạng Guid (Khóa chính)
        /// </summary>
        [Key]
        public Guid EmailTemplateId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Mã mẫu email (Ví dụ: forgot_password, welcome...)
        /// </summary>
        [DisplayName("Mã mẫu email")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(100, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        public string TemplateCode { get; set; } = string.Empty;

        /// <summary>
        /// Tiêu đề email của mẫu
        /// </summary>
        [DisplayName("Tiêu đề email")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(255, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Nội dung chi tiết của mẫu email (HTML)
        /// </summary>
        [DisplayName("Nội dung email")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public string Content { get; set; } = string.Empty;
    }
}

