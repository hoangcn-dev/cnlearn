using HoangCN.Core.Common.Attributes;
using HoangCN.Core.Common.Base;
using HoangCN.LearnMS.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HoangCN.LearnMS.Entities
{
    /// <summary>
    /// Thực thể đại diện cho các lần lưu của user
    /// </summary>
    [Index(nameof(LearnMsUserId), nameof(TargetId), IsUnique = true)]
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
        [FK(TargetEntity = typeof(LearnMsUser))]
        public Guid LearnMsUserId { get; set; }

        /// <summary>
        /// Kiểu nội dung lưu
        /// </summary>
        public SaveType SaveType { get; set; }
    }

    public class UserSavedMappingConfiguration : IEntityTypeConfiguration<UserSavedMapping>
    {
        public void Configure(EntityTypeBuilder<UserSavedMapping> builder)
        {
            builder.ToTable("UserSavedMapping");
            builder.HasOne<LearnMsUser>()
                   .WithMany()
                   .HasForeignKey(us => us.LearnMsUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
