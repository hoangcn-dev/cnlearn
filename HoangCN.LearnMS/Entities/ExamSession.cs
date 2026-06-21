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
    [Table("ExamSession")]
    public class ExamSession : BaseEntity
    {
        [Key]
        [DisplayName("Mã phiên làm bài")]
        public Guid SessionId { get; set; }

        [DisplayName("Mã học viên")]
        public Guid CandidateId { get; set; }

        [DisplayName("Mã đợt thi")]
        public Guid QuizId { get; set; }

        [DisplayName("Thời gian bắt đầu")]
        public DateTime StartedAt { get; set; }

        [DisplayName("Thời gian gửi heartbeat cuối")]
        public DateTime LastHeartbeatAt { get; set; }

        [DisplayName("Đang hoạt động")]
        public bool IsActive { get; set; }

        [DisplayName("Trạng thái")]
        public ExamSessionStatus Status { get; set; }

        [DisplayName("Số lần chuyển tab")]
        public int BlurCount { get; set; }

        [DisplayName("Số lần thoát toàn màn hình")]
        public int FullscreenExitCount { get; set; }
    }

    public class ExamSessionConfiguration : IEntityTypeConfiguration<ExamSession>
    {
        public void Configure(EntityTypeBuilder<ExamSession> builder)
        {
            builder.ToTable("ExamSession");
            builder.HasIndex(es => new { es.CandidateId, es.QuizId }).IsUnique();

            builder.HasOne<LearnMsUser>()
                   .WithMany()
                   .HasForeignKey(es => es.CandidateId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Quiz>()
                   .WithMany()
                   .HasForeignKey(es => es.QuizId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
