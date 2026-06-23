using HoangCN.Core.Common.Attributes;
using HoangCN.Core.Common.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoangCN.MainSystem.Entities
{
    /// <summary>
    /// Người dùng hệ thống
    /// </summary>
    [Index(nameof(UserName), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class User : BaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid UserId { get; set; }

        /// <summary>
        /// Tên tài khoản người dùng
        /// </summary>
        [DisplayName("Tên tài khoản")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(100, ErrorMessage = "{0} không được phép vượt quá {1} ký tự.")]
        [CheckExist(MustExist = false, ErrorMessage = "{0} đã được sử dụng.")]
        public string UserName { get; set; }

        /// <summary>
        /// Mã người dùng
        /// </summary>
        [DisplayName("Mã tài khoản")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(20, ErrorMessage = "{0} không được phép vượt quá {1} ký tự.")]
        [CheckExist(MustExist = false, ErrorMessage = "{0} đã được sử dụng.")]
        public string UserCode { get; set; }

        /// <summary>
        /// Tên hiển thị người dùng
        /// </summary>
        [DisplayName("Tên hiển thị")]
        [StringLength(100, ErrorMessage = "{0} không được phép vượt quá {1} ký tự.")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Tài khoản email
        /// </summary>
        [DisplayName("Email")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(100, ErrorMessage = "{0} không được phép vượt quá {1} ký tự.")]
        [CheckExist(MustExist = false, ErrorMessage = "{0} đã được sử dụng.")]
        public string Email { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [DisplayName("Mật khẩu")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(255, ErrorMessage = "{0} không được phép vượt quá {1} ký tự.")]
        public string Password { get; set; }

        [StringLength(255)]
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Tài khoản có đang được kích hoạt hay không
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Đã xác thực hay chưa (Xác thực qua email)
        /// </summary>
        public bool IsVerified { get; set; } = false;

        /// <summary>
        /// Id ảnh đại diện
        /// </summary>
        public Guid? AvatarImageFileId { get; set; }

        /// <summary>
        /// Lần cuối đăng nhập
        /// </summary>
        public DateTime LastLogin { get; set; }

        /// <summary>
        /// ID Vai trò (Khóa ngoại đến bảng Role)
        /// </summary>
        [DisplayName("Quyền")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [FK(TargetEntity = typeof(Role))]
        public Guid RoleId { get; set; }

        /// <summary>
        /// Đối tượng vai trò
        /// </summary>
        [ForeignKey(nameof(RoleId))]
        [NotMapped]
        public virtual Role Role { get; set; }
    }
}

