using HoangCN.Core.Common.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoangCN.LearnMS.Entities
{
    /// <summary>
    /// Thực thể lưu lượt làm bài / luyện tập của người dùng (bao gồm câu hỏi lẻ, đề thi, kỳ thi)
    /// </summary>
    [Table("ExamAttempt")]
    public class ExamAttempt : BaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid ExamAttemptId { get; set; }

        /// <summary>
        /// Mã định danh người dùng (Khóa ngoại)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Mã người dùng")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Loại lượt làm: "question" (câu hỏi lẻ), "exam" (luyện tập đề thi), "quiz" (kỳ thi/bài kiểm tra thực tế)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(50)]
        [DisplayName("Loại lượt làm")]
        public string AttemptType { get; set; } = "exam";

        /// <summary>
        /// Định danh câu hỏi lẻ (nếu AttemptType = "question")
        /// </summary>
        [DisplayName("Mã câu hỏi lẻ")]
        public Guid? QuestionId { get; set; }

        /// <summary>
        /// Định danh đề thi (nếu AttemptType = "exam")
        /// </summary>
        [DisplayName("Mã đề thi")]
        public Guid? ExamId { get; set; }

        /// <summary>
        /// Định danh kỳ thi (nếu AttemptType = "quiz")
        /// </summary>
        [DisplayName("Mã kỳ thi")]
        public Guid? QuizId { get; set; }

        /// <summary>
        /// Điểm số đạt được
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayName("Điểm số")]
        public decimal Score { get; set; } = 0;

        /// <summary>
        /// Tổng số câu hỏi trong bài thi
        /// </summary>
        [DisplayName("Tổng số câu hỏi")]
        public int TotalQuestions { get; set; } = 0;

        /// <summary>
        /// Số câu trả lời đúng
        /// </summary>
        [DisplayName("Số câu đúng")]
        public int CorrectCount { get; set; } = 0;

        /// <summary>
        /// Thời gian làm bài (tính bằng giây)
        /// </summary>
        [DisplayName("Thời gian làm bài (giây)")]
        public int Duration { get; set; } = 0;

        /// <summary>
        /// Thời điểm bắt đầu làm bài
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Thời gian bắt đầu")]
        public DateTime StartedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Thời điểm kết thúc / nộp bài
        /// </summary>
        [DisplayName("Thời gian kết thúc")]
        public DateTime? FinishedDate { get; set; }
    }
}
