using HoangCN.Core.Common.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoangCN.LearnMS.Entities
{
    /// <summary>
    /// Thực thể lưu trữ thông tin cá nhân của học viên/người dùng riêng biệt trong dịch vụ LearnMS
    /// </summary>
    [Table("LearnMsUser")]
    public class LearnMsUser : BaseEntity
    {
        /// <summary>
        /// Khóa chính trùng khớp với UserId từ hệ thống Identity chính (MainSystem)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("Mã người dùng")]
        public Guid LearnMsUserId { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(150, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        [DisplayName("Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Email liên hệ (không được phép chỉnh sửa sau khi tạo)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [EmailAddress(ErrorMessage = "{0} không đúng định dạng email.")]
        [StringLength(150, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        [DisplayName("Email liên hệ")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Số điện thoại
        /// </summary>
        [StringLength(20, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        [Phone(ErrorMessage = "{0} không đúng định dạng số điện thoại.")]
        [DisplayName("Số điện thoại")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Giới thiệu bản thân
        /// </summary>
        [StringLength(1000, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        [DisplayName("Giới thiệu bản thân")]
        public string? Biography { get; set; }
    }
}
