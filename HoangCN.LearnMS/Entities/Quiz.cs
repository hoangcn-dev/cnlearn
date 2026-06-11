using HoangCN.Core.Common.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoangCN.LearnMS.Entities
{
    /// <summary>
    /// Thực thể quản lý đợt thi / kỳ thi / bài kiểm tra thực tế
    /// </summary>
    [Table("Quiz")]
    public class Quiz : BaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        [DisplayName("Mã kỳ thi")]
        public Guid QuizId { get; set; }

        /// <summary>
        /// Tên kỳ thi
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(255, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        [DisplayName("Tên kỳ thi")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Đối tượng/Lớp học tham gia
        /// </summary>
        [StringLength(255, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        [DisplayName("Đối tượng tham gia")]
        public string? TargetGroup { get; set; }

        /// <summary>
        /// Nguồn câu hỏi: "exam" (từ đề thi cố định), "direct" (cấu hình quy tắc sinh ngẫu nhiên)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(50)]
        [DisplayName("Nguồn câu hỏi")]
        public string SourceType { get; set; } = "exam";

        /// <summary>
        /// Khóa ngoại trỏ đến đề thi (Null nếu SourceType = "direct")
        /// </summary>
        [DisplayName("Đề thi liên kết")]
        public Guid? ExamId { get; set; }

        /// <summary>
        /// Thời gian bắt đầu kỳ thi
        /// </summary>
        [DisplayName("Thời gian bắt đầu")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Thời gian kết thúc kỳ thi
        /// </summary>
        [DisplayName("Thời gian kết thúc")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Trạng thái bản nháp
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Trạng thái nháp")]
        public bool IsDraft { get; set; } = false;

        /// <summary>
        /// Người tạo kỳ thi
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Người tạo kỳ thi")]
        public Guid UserId { get; set; }

        #region Cấu hình chống gian lận (Anti-Cheat Settings)

        /// <summary>
        /// Khóa trình duyệt (Chống chuyển tab)
        /// </summary>
        [DisplayName("Khóa trình duyệt")]
        public bool LockBrowser { get; set; } = false;

        /// <summary>
        /// Tự động đảo câu hỏi
        /// </summary>
        [DisplayName("Tự động đảo câu hỏi")]
        public bool ShuffleQuestions { get; set; } = false;

        /// <summary>
        /// Cấm copy/paste
        /// </summary>
        [DisplayName("Cấm copy/paste")]
        public bool DisableCopyPaste { get; set; } = false;

        /// <summary>
        /// Yêu cầu toàn màn hình (Fullscreen)
        /// </summary>
        [DisplayName("Yêu cầu toàn màn hình")]
        public bool Fullscreen { get; set; } = false;

        /// <summary>
        /// Yêu cầu Webcam
        /// </summary>
        [DisplayName("Yêu cầu Webcam")]
        public bool Webcam { get; set; } = false;

        /// <summary>
        /// Giới hạn dải IP
        /// </summary>
        [DisplayName("Giới hạn dải IP")]
        public bool IpLimit { get; set; } = false;

        #endregion

        #region Tiện ích bổ sung

        /// <summary>
        /// Cho phép bù giờ vào muộn
        /// </summary>
        [DisplayName("Cho phép bù giờ vào muộn")]
        public bool AllowLateJoin { get; set; } = true;

        /// <summary>
        /// Cho phép nộp bài muộn
        /// </summary>
        [DisplayName("Cho phép nộp bài muộn")]
        public bool AllowLateSubmit { get; set; } = false;

        /// <summary>
        /// Công khai bảng xếp hạng kết quả
        /// </summary>
        [DisplayName("Công khai bảng xếp hạng")]
        public bool PublicLeaderboard { get; set; } = true;

        /// <summary>
        /// Gửi email báo cáo kết quả tự động
        /// </summary>
        [DisplayName("Gửi email báo cáo")]
        public bool SendEmailReport { get; set; } = true;

        #endregion

        #region Cấu hình sinh câu hỏi ngẫu nhiên trực tiếp (Dùng khi SourceType = "direct")

        /// <summary>
        /// Môn học cần lấy câu hỏi ngẫu nhiên
        /// </summary>
        [DisplayName("Môn học lấy ngẫu nhiên")]
        public Guid? DirectCategoryId { get; set; }

        /// <summary>
        /// Tổng số câu hỏi ngẫu nhiên cần lấy
        /// </summary>
        [DisplayName("Tổng số câu hỏi ngẫu nhiên")]
        public int? DirectTotalQuestions { get; set; }

        /// <summary>
        /// Số câu hỏi mức độ Dễ
        /// </summary>
        [DisplayName("Số câu dễ")]
        public int DirectEasyCount { get; set; } = 0;

        /// <summary>
        /// Số câu hỏi mức độ Trung bình
        /// </summary>
        [DisplayName("Số câu trung bình")]
        public int DirectMediumCount { get; set; } = 0;

        /// <summary>
        /// Số câu hỏi mức độ Khó
        /// </summary>
        [DisplayName("Số câu khó")]
        public int DirectHardCount { get; set; } = 0;

        #endregion
    }
}
