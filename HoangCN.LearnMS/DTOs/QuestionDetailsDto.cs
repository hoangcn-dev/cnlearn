using System;
using System.Collections.Generic;

namespace HoangCN.LearnMS.DTOs
{
    /// <summary>
    /// DTO chi tiết của câu hỏi phục vụ hiển thị và lưu trữ ở Client
    /// </summary>
    public class QuestionDetailsDto
    {
        public Guid Id { get; set; }
        public string? Slug { get; set; }
        public string? StringContent { get; set; }
        public string? Explanation { get; set; }
        public int Level { get; set; }
        public int Type { get; set; }
        public int AccessType { get; set; }
        public List<Guid> CategoryIds { get; set; } = new();
        public List<AnswerDetailsDto> Answers { get; set; } = new();
        public bool IsMyCreated { get; set; }
    }

    /// <summary>
    /// DTO chi tiết đáp án câu hỏi
    /// </summary>
    public class AnswerDetailsDto
    {
        public Guid QuestionAnswerId { get; set; }
        public string? StringContent { get; set; }
        public bool IsCorrectAnswer { get; set; }
    }
}
