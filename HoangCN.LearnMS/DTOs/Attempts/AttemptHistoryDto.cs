using System;

namespace HoangCN.LearnMS.DTOs.Attempts
{
    public class AttemptHistoryDto
    {
        public Guid ExamAttemptId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AttemptType { get; set; } = string.Empty;
        public Guid? RelatedId { get; set; }
        public DateTime FinishedDate { get; set; }
        public int CorrectCount { get; set; }
        public int TotalQuestions { get; set; }
        public decimal Score { get; set; }
        public int Duration { get; set; }
    }
}
