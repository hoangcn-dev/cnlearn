using HoangCN.Core.Common.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoangCN.LearnMS.Entities
{
    /// <summary>
    /// Thực thể lưu chi tiết câu trả lời của người dùng cho từng câu hỏi trong lượt làm bài
    /// </summary>
    [Table("ExamAttemptDetail")]
    public class ExamAttemptDetail : BaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid ExamAttemptDetailId { get; set; }

        /// <summary>
        /// Mã định danh lượt làm bài (Khóa ngoại)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Mã lượt làm bài")]
        public Guid ExamAttemptId { get; set; }

        /// <summary>
        /// Mã định danh câu hỏi (Khóa ngoại)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Mã câu hỏi")]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Danh sách ID của các phương án trả lời được chọn (ngăn cách bởi dấu phẩy ",")
        /// </summary>
        [StringLength(500)]
        [DisplayName("Các phương án đã chọn")]
        public string? SelectedAnswerIds { get; set; }

        /// <summary>
        /// Cho biết câu trả lời có đúng hay không
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Trả lời đúng")]
        public bool IsCorrect { get; set; } = false;
    }

    public class ExamAttemptDetailConfiguration : IEntityTypeConfiguration<ExamAttemptDetail>
    {
        public void Configure(EntityTypeBuilder<ExamAttemptDetail> builder)
        {
            builder.ToTable("ExamAttemptDetail");
            builder.HasIndex(ead => ead.ExamAttemptId);
            builder.HasIndex(ead => ead.QuestionId);

            builder.HasOne<ExamAttempt>()
                   .WithMany()
                   .HasForeignKey(ead => ead.ExamAttemptId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Question>()
                   .WithMany()
                   .HasForeignKey(ead => ead.QuestionId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
