using HoangCN.Core.Common.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.MainSystem.Entities
{
    /// <summary>
    /// Bảng quản lý thông tin các file tài nguyên được upload
    /// </summary>
    public class ResourceFile : BaseEntity
    {
        /// <summary>
        /// Khóa chính (Trùng khớp với ResourceFileId)
        /// </summary>
        [Key]
        public Guid ResourceFileId { get; set; }

        /// <summary>
        /// Tên nguyên bản của file khi upload
        /// </summary>
        [DisplayName("Tên file gốc")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(255, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        public string OriginName { get; set; }

        /// <summary>
        /// Định dạng MIME type của file
        /// </summary>
        [DisplayName("MIME Type")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(100, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        public string Type { get; set; }

        /// <summary>
        /// Kích thước file tính bằng byte
        /// </summary>
        [DisplayName("Dung lượng file")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public long Size { get; set; }

        /// <summary>
        /// Đường dẫn link hoặc URL để truy cập file
        /// </summary>
        [DisplayName("Đường dẫn Url")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(500, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        public string Url { get; set; }
    }
}

