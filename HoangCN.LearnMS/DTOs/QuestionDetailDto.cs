using HoangCN.LearnMS.Enums;

namespace HoangCN.LearnMS.DTOs
{
    /// <summary>
    /// DTO chi tiết của câu hỏi phục vụ hiển thị và lưu trữ ở Client
    /// </summary>
    public class QuestionDetailDto

    {
        public Guid QuestionId { get; set; }
        public string? Slug { get; set; }
        public string? StringContent { get; set; }
        public string? Explaination { get; set; }
        public QuestionLevel Level { get; set; }
        public QuestionType Type  { get; set; }
        public QuestionAccessType AccessType { get; set; }
        public Guid QuestionCategoryName { get; set; }
        public Guid QuestionCategoryId { get; set; }
        public int AttemptCount { get; set; } = 0;
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public bool IsInBank { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
