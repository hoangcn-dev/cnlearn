using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HoangCN.LearnMS.DTOs
{
    public class ExamQuestionDto
    {
        public Guid ExamQuestionId { get; set; }
        public string? StringContent { get; set; }
        public int OrderInExam { get; set; }
        public string? Explaination { get; set; }
        public QuestionLevel Level { get; set; }
        public QuestionType Type { get; set; }

        [JsonIgnore]
        public string AnswersJsonData { get; set; }

        [NotMapped]
        public List<QuestionAnswerDto> Answers => !string.IsNullOrEmpty(AnswersJsonData) ?
            JsonSerializer.Deserialize<List<QuestionAnswerDto>>(AnswersJsonData)! : [];
    }
}
