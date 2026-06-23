using HoangCN.Core.Common.Attributes;
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
    /// <summary>
    /// Thực thể quản lý thông tin Đề thi (tĩnh)
    /// </summary>
    [Table("Exam")]
    public class Exam : BaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        [DisplayName("Mã đề")]
        public Guid ExamId { get; set; }

        /// <summary>
        /// Tên đề thi
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(255, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        [DisplayName("Tên đề")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả chi tiết đề thi
        /// </summary>
        [StringLength(1000, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        [DisplayName("Mô tả đề")]
        public string? Description { get; set; }

        /// <summary>
        /// Danh mục môn học / chuyên đề
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Danh mục")]
        [FK(TargetEntity = typeof(QuestionCategory))]
        public Guid QuestionCategoryId { get; set; }

        /// <summary>
        /// Thời gian làm bài (phút)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Thời gian làm bài")]
        public int DurationMin { get; set; }

        /// <summary>
        /// Phạm vi truy cập (0: Riêng tư, 1: Công khai)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Phạm vi truy cập")]
        public ExamAccessType AccessType { get; set; }

        /// <summary>
        /// Trạng thái bản nháp
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Trạng thái nháp")]
        public bool IsDraft { get; set; } = false;

        /// <summary>
        /// Dữ liệu nháp
        /// </summary>
        [DisplayName("Dữ liệu nháp")]
        public string? DraftData { get; set; }

        /// <summary>
        /// Người tạo đề
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Người tạo đề")]
        public Guid LearnMsUserId { get; set; }

        /// <summary>
        /// Đóng góp vào ngân hàng câu hỏi chung 
        /// </summary>
        [DisplayName("Đóng góp câu hỏi mới vào ngân hàng câu hỏi")]
        public bool ContributeToBank { get; set; } = true;

        [NotMapped]
        public List<ExamQuestion> Questions { get; set; }
    }

    public class ExamConfiguration : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder.ToTable("Exam");
            builder.HasIndex(e => e.QuestionCategoryId);
            builder.HasIndex(e => e.LearnMsUserId);

            builder.HasOne<QuestionCategory>()
                   .WithMany()
                   .HasForeignKey(e => e.QuestionCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<LearnMsUser>()
                   .WithMany()
                   .HasForeignKey(e => e.LearnMsUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
