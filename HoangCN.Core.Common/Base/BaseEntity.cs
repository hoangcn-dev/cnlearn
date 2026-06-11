using HoangCN.Core.Common.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HoangCN.Core.Common.Base
{
    /// <summary>
    /// Lớp cơ sở cho các entity, gồm các thuộc tính chung
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Người tạo
        /// </summary>
        [BindNever]
        [Required]
        [MaxLength(100)]
        [JsonIgnore]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Thời điểm tạo
        /// </summary>
        [BindNever]
        [Required]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Người thay đổi cuối
        /// </summary>
        [BindNever]
        [MaxLength(100)]
        [JsonIgnore]
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Thời điểm lần cuối thay đổi
        /// </summary>
        [BindNever]
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Trạng thái của entity
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public ModelState State { get; set; } = ModelState.None;

        /// <summary>
        /// Đã xóa mềm
        /// </summary>
        [BindNever]
        [JsonIgnore]
        public bool IsDeleted { get; set; } = false;
    }
}

