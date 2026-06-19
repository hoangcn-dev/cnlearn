using HoangCN.Core.Common.Attributes;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Enums;

namespace HoangCN.LearnMS.DTOs
{
    /// <summary>
    /// DTO chi tiết của câu hỏi phục vụ hiển thị và lưu trữ ở Client
    /// </summary>
    public class BankQuestionDto
    {
        public Guid QuestionId { get; set; }
        public string? QuestionSlug { get; set; }
        public string? StringContent { get; set; }
        public string? Explaination { get; set; }
        public QuestionLevel Level { get; set; }
        public QuestionType Type  { get; set; }
        public QuestionAccessType AccessType { get; set; }

        [ForeignTable(EntityType = typeof(QuestionCategory))]
        public string QuestionCategoryName { get; set; }

        [ForeignTable(EntityType = typeof(QuestionCategory))]
        public Guid QuestionCategoryId { get; set; }


        public int AttemptCount { get; set; } = 0;
        public Guid LearnMsUserId { get; set; }

        [ForeignTable(EntityType = typeof(LearnMsUser))]
        public string FullName { get; set; }

        public bool IsInBank { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
