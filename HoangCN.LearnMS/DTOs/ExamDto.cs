using HoangCN.Core.Common.Attributes;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Enums;

namespace HoangCN.LearnMS.DTOs
{
    public class ExamDto
    {
        public Guid ExamId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid QuestionCategoryId { get; set; }

        [ForeignTable(EntityType = typeof(QuestionCategory))]
        public string QuestionCategoryName { get; set; }

        public int DurationMin { get; set; }
        public ExamAccessType AccessType { get; set; }
        public bool IsDraft { get; set; }
        public Guid LearnMsUserId { get; set; }

        [ForeignTable(EntityType = typeof(LearnMsUser), ColumnName = nameof(LearnMsUser.FullName))]
        public string AuthorFullname { get; set; }
    }
}
