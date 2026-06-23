using HoangCN.Core.Common.Attributes;
using HoangCN.Core.Common.Base;
using HoangCN.LearnMS.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace HoangCN.LearnMS.Entities
{
    /// <summary>
    /// Bảng lưu các câu hỏi có trong đề thi
    /// </summary>
    [Table("ExamQuestion")]
    public class ExamQuestion : BaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid ExamQuestionId { get; set; }

        /// <summary>
        /// Nội dung câu hỏi bằng chữ
        /// </summary>
        [DisplayName("Nội dung câu hỏi")]
        public string? StringContent { get; set; }

        /// <summary>
        /// Thứ tự trong đề
        /// </summary>
        [DisplayName("Thứ tự trong đề")]
        public int OrderInExam { get; set; }

        /// <summary>
        /// Giải thích chi tiết đáp án câu hỏi
        /// </summary>
        [DisplayName("Giải thích")]
        public string? Explaination { get; set; }

        /// <summary>
        /// Mức độ khó của câu hỏi
        /// </summary>
        [DisplayName("Mức độ")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public QuestionLevel Level { get; set; } = QuestionLevel.Easy;

        /// <summary>
        /// Loại câu hỏi (Một lựa chọn hay Nhiều lựa chọn)
        /// </summary>
        [DisplayName("Loại câu hỏi")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public QuestionType Type { get; set; } = QuestionType.SingleChoice;

        /// <summary>
        /// Đề chưa câu hỏi
        /// </summary>
        [DisplayName("Đề thi chứa câu hỏi")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public Guid ExamId { get; set; }

        ///// <summary>
        ///// Định danh danh mục câu hỏi (Khóa ngoại)
        ///// </summary>
        //[DisplayName("Danh mục")]
        //[Required(ErrorMessage = "{0} không được phép để trống.")]
        //[FK(TargetEntity = typeof(QuestionCategory))]
        //public Guid QuestionCategoryId { get; set; }

        /// <summary>
        /// Danh sách đáp án đi kèm câu hỏi (Được serialize thành JSON dưới DB)
        /// </summary>
        [DisplayName("Danh sách đáp án")]
        public List<QuestionAnswer> Answers { get; set; } = [];

        /// <summary>
        /// Danh sách ID các đáp án đúng
        /// </summary>
        [DisplayName("Mã đáp án đúng")]
        public List<Guid>? CorrectKeys { get; set; }
    }

    public class ExamQuestionConfiguration : IEntityTypeConfiguration<ExamQuestion>
    {
        public void Configure(EntityTypeBuilder<ExamQuestion> builder)
        {
            builder.ToTable("ExamQuestion"); 
            builder.HasIndex(eq => eq.ExamId);

            builder.HasOne<Exam>()
                   .WithMany()
                   .HasForeignKey(eq => eq.ExamId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình Value Converter cho Answers
            builder.Property(q => q.Answers)
                   .HasColumnName(nameof(Question.Answers) + "JsonData")
                   .HasConversion(
                       v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                       v => JsonSerializer.Deserialize<List<QuestionAnswer>>(v, (JsonSerializerOptions)null) ?? new List<QuestionAnswer>()
                   )
                   .HasColumnType("longtext");

            // Cấu hình Value Converter cho CorrectKeys
            builder.Property(q => q.CorrectKeys)
                   .HasColumnName(nameof(Question.CorrectKeys) + "JsonData")
                   .HasConversion(
                       v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                       v => string.IsNullOrEmpty(v) ? new List<Guid>() : JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null) ?? new List<Guid>()
                   )
                   .HasColumnType("longtext");
        }
    }
}
