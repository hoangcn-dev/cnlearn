using System;
using System.Collections.Generic;

namespace HoangCN.LearnMS.DTOs
{
    public class QuestionCheckResultDto
    {
        public bool IsCorrect { get; set; }
        public string? Explanation { get; set; }
        public List<Guid>? CorrectAnswerIds { get; set; }
    }
}
