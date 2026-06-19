using HoangCN.Core.Common.Base;
using HoangCN.LearnMS.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HoangCN.LearnMS.Entities
{
    /// <summary>
    /// Thực thể đại diện cho các lần lưu của user
    /// </summary>
    [Index(nameof(UserId), nameof(TargetId), IsUnique = true)]
    public class UserSavedMapping : BaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid UserSavedMappingId { get; set; }

        /// <summary>
        /// Id của đối tượng được lưu
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// Id người dùng lưu
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Kiểu nội dung lưu
        /// </summary>
        public SaveType SaveType { get; set; }

        [ForeignKey(nameof(UserId))]
        public LearnMsUser User { get; set; }
    }
}
