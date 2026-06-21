using HoangCN.Core.Common.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.LearnMS.Entities
{
    /// <summary>
    /// Bảng lưu trữ liên kết ảnh, video, hoặc âm thanh cho câu hỏi
    /// </summary>
    public class QuestionMedia : BaseEntity
    {
        /// <summary>
        /// Khóa chính (Mã file tài nguyên tương ứng, trùng khớp với ResourceFileId)
        /// </summary>
        [Key]
        public Guid FileId { get; set; }

        /// <summary>
        /// Mã câu hỏi sở hữu media này
        /// </summary>
        [DisplayName("Mã câu hỏi")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Nhãn hiển thị của file phương tiện (Ảnh câu hỏi, Video mô tả...)
        /// </summary>
        [DisplayName("Nhãn hiển thị")]
        [StringLength(255, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        public string? Label { get; set; }

        /// <summary>
        /// Thứ tự hiển thị phương tiện của câu hỏi
        /// </summary>
        [DisplayName("Thứ tự hiển thị")]
        public int OrderInList { get; set; } = 0;
    }

    public class QuestionMediaConfiguration : IEntityTypeConfiguration<QuestionMedia>
    {
        public void Configure(EntityTypeBuilder<QuestionMedia> builder)
        {
            builder.ToTable("QuestionMedia");
            builder.HasIndex(qm => qm.QuestionId);

            builder.HasOne<Question>()
                   .WithMany()
                   .HasForeignKey(qm => qm.QuestionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
