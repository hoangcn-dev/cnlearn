using System;
using System.Collections.Generic;

namespace HoangCN.LearnMS.DTOs
{
    /// <summary>
    /// DTO nhận yêu cầu lưu đề thi và danh sách câu hỏi đi kèm
    /// </summary>
    public class ExamSaveDto
    {
        public Guid? ExamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public int Duration { get; set; } = 45;
        public int AccessType { get; set; } = 1;
        public bool IsDraft { get; set; } = false;
        public bool ContributeToBank { get; set; } = true;
        public List<QuestionDetailDto> Questions { get; set; } = new();
    }
}
