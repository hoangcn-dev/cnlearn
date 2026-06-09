using HoangCN.Common.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.MainSystem.Entities
{
    /// <summary>
    /// Bảng phân quyền lưu các vai trò trong hệ thống
    /// </summary>
    [Index(nameof(RoleName), IsUnique = true)]
    public class Role : BaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid RoleId { get; set; }

        /// <summary>
        /// Tên quyền
        /// </summary>
        [DisplayName("Tên quyền")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        [StringLength(20, ErrorMessage = "{0} dài tối đa {1} kí tự")]
        public string RoleName { get; set; }
    }
}
