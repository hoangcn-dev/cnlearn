using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.LearnMS.DTOs.Attempts
{
    public class ExamAttemptSubmitDto
    {
        [Required(ErrorMessage = "ExamId là bắt buộc")]
        public Guid ExamId { get; set; }

        public Guid? QuizId { get; set; }

        [Required(ErrorMessage = "Loại làm bài (AttemptType) là bắt buộc")]
        public string AttemptType { get; set; } = "exam"; // "practice", "exam", "quiz"

        public int TimeSpentSeconds { get; set; } = 0;

        public List<QuestionAnswerSubmitDto> Answers { get; set; } = new List<QuestionAnswerSubmitDto>();
    }

    public class QuestionAnswerSubmitDto
    {
        [Required(ErrorMessage = "QuestionId là bắt buộc")]
        public Guid QuestionId { get; set; }

        public List<Guid> SelectedAnswerIds { get; set; } = new List<Guid>();
    }

    public class ExamAttemptResultDto
    {
        public Guid ExamAttemptId { get; set; }
        public decimal Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }
        public int Duration { get; set; }
        
        // Trả về danh sách chi tiết cùng với đáp án đúng để client render kết quả
        public List<QuestionAttemptResultDto> Details { get; set; } = new List<QuestionAttemptResultDto>();
    }

    public class QuestionAttemptResultDto
    {
        public Guid QuestionId { get; set; }
        public bool IsCorrect { get; set; }
        public List<Guid> SelectedAnswerIds { get; set; } = new List<Guid>();
        
        // Trả về danh sách đáp án thực sự đúng (Id của các Answer)
        public List<Guid> CorrectAnswerIds { get; set; } = new List<Guid>();
        public string Explanation { get; set; } = string.Empty;
    }
}
