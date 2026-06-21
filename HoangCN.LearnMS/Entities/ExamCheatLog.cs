using HoangCN.Core.Common.Base;
using HoangCN.LearnMS.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoangCN.LearnMS.Entities
{
    [Table("ExamCheatLog")]
    public class ExamCheatLog : BaseEntity
    {
        [Key]
        [DisplayName("Mã nhật ký")]
        public Guid LogId { get; set; }

        [DisplayName("Mã phiên làm bài")]
        public Guid SessionId { get; set; }

        [DisplayName("Thời gian vi phạm")]
        public DateTime Timestamp { get; set; }

        [DisplayName("Loại vi phạm")]
        public ExamViolationType ViolationType { get; set; }
    }

    public class ExamCheatLogConfiguration : IEntityTypeConfiguration<ExamCheatLog>
    {
        public void Configure(EntityTypeBuilder<ExamCheatLog> builder)
        {
            builder.ToTable("ExamCheatLog");
            builder.HasIndex(ecl => ecl.SessionId);

            builder.HasOne<ExamSession>()
                   .WithMany()
                   .HasForeignKey(ecl => ecl.SessionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
