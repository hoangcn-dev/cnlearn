using System;
using System.Collections.Generic;

namespace HoangCN.LearnMS.DTOs
{
    public class QuestionCheckDto
    {
        public Guid QuestionId { get; set; }
        public List<Guid>? SelectedAnswerIds { get; set; }
    }
}
