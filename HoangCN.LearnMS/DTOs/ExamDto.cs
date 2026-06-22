using HoangCN.Core.Common.Attributes;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoangCN.LearnMS.DTOs
{
    public class ExamDto
    {
        public Guid ExamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }

        [ForeignTable(EntityType = typeof(QuestionCategory))]
        public string CategoryName { get; set; }

        public int DurationMin { get; set; }
        public ExamAccessType AccessType { get; set; }
        public bool IsDraft { get; set; }
        public Guid LearnMsUserId { get; set; }

        [ForeignTable(EntityType = typeof(LearnMsUser), ColumnName = nameof(LearnMsUser.FullName))]
        public string AuthorFullname { get; set; }

        public List<ExamQuestion> Questions { get; set; }
    }
}
