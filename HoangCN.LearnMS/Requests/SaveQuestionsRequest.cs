using HoangCN.Core.Common.Attributes;
using HoangCN.Core.Common.Enums;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.LearnMS.Requests
{
    public class SaveQuestionsRequest
    {
        /// <summary>
        /// Danh sách câu hỏi chi tiết kèm đáp án và danh mục
        /// </summary>
        [DisplayName("Danh sách câu hỏi")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public List<SaveQuestionsDto> Questions { get; set; }

        /// <summary>
        /// Cho biết câu hỏi này đã được đưa vào ngân hàng câu hỏi chung/cá nhân hay chưa
        /// </summary>
        [DisplayName("Đóng góp vào ngân hàng")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public bool IsInBank { get; set; } = false;

        /// <summary>
        /// Quyền truy cập của câu hỏi
        /// </summary>
        [DisplayName("Quyền truy cập")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public QuestionAccessType AccessType { get; set; } = QuestionAccessType.Public;
    }

    public class SaveQuestionsDto
    {
        /// <summary>
        /// Nội dung câu hỏi bằng chữ
        /// </summary>
        [DisplayName("Nội dung câu hỏi")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public string StringContent { get; set; }


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
        /// Cho biết câu hỏi này đã được đưa vào ngân hàng câu hỏi chung/cá nhân hay chưa
        /// </summary>
        [DisplayName("Đáp án")]
        public List<SaveAnswerDto> Answers { get; set; }


        /// <summary>
        /// Cho biết câu hỏi này đã được đưa vào ngân hàng câu hỏi chung/cá nhân hay chưa
        /// </summary>
        [DisplayName("Danh mục")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        [CheckExist(MustExist = true, TargetEntity = typeof(QuestionCategory), ErrorMessage = "Danh mục không tồn tại trong hệ thống.")]
        public Guid QuestionCategoryId { get; set; }
    }

    public class SaveAnswerDto
    {
        /// <summary>
        /// Cho biết câu hỏi này đã được đưa vào ngân hàng câu hỏi chung/cá nhân hay chưa
        /// </summary>
        [DisplayName("Nội dung đáp án")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public string StringContent { get; set; }


        /// <summary>
        /// Cho biết câu hỏi này đã được đưa vào ngân hàng câu hỏi chung/cá nhân hay chưa
        /// </summary>
        [DisplayName("Đánh dấu đáp án đúng")]
        [Required(ErrorMessage = "{0} không được phép để trống.")]
        public bool IsCorrectAnswer { get; set; }
    }
}
