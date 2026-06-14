using HoangCN.Core.Common.Base;
using HoangCN.LearnMS.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoangCN.LearnMS.Entities
{
    [Table("ExamCheatLog")]
    public class ExamCheatLog : BaseEntity
    {
        [Key]
        [DisplayName("Mã nhật ký")]
        public Guid LogId { get; set; }

        [DisplayName("Mã phiên làm bài")]
        public Guid SessionId { get; set; }

        [DisplayName("Thời gian vi phạm")]
        public DateTime Timestamp { get; set; }

        [DisplayName("Loại vi phạm")]
        public ExamViolationType ViolationType { get; set; }
    }
}
