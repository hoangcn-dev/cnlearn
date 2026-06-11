using HoangCN.Core.Common.Base;
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
        [DisplayName("Mã đề thi")]
        public Guid ExamId { get; set; }

        /// <summary>
        /// Tên đề thi
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [StringLength(255, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        [DisplayName("Tên đề thi")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả chi tiết đề thi
        /// </summary>
        [StringLength(1000, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        [DisplayName("Mô tả đề thi")]
        public string? Description { get; set; }

        /// <summary>
        /// Danh mục môn học / chuyên đề
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Danh mục môn học")]
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Thời gian làm bài (phút)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Thời gian làm bài")]
        public int Duration { get; set; } = 45;

        /// <summary>
        /// Phạm vi truy cập (0: Riêng tư, 1: Công khai)
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Phạm vi truy cập")]
        public int AccessType { get; set; } = 1;

        /// <summary>
        /// Trạng thái bản nháp
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Trạng thái nháp")]
        public bool IsDraft { get; set; } = false;

        /// <summary>
        /// Người tạo đề
        /// </summary>
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [DisplayName("Người tạo đề")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Đóng góp vào ngân hàng câu hỏi chung
        /// </summary>
        [DisplayName("Đóng góp ngân hàng câu hỏi")]
        public bool ContributeToBank { get; set; } = true;

        /// <summary>
        /// Đề thi được sinh ngầm tự động phục vụ riêng cho Kỳ thi/Bài kiểm tra
        /// Nếu là true: Không đưa vào Ngân hàng đề thi cá nhân/công khai và không tự động đóng góp câu hỏi
        /// </summary>
        [DisplayName("Sinh ngầm từ kỳ thi")]
        public bool IsQuizSource { get; set; } = false;
    }
}
