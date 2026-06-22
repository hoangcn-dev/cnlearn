using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HoangCN.LearnMS.DTOs
{
    public class QuestionAnswerDto
    {
        public Guid QuestionAnswerId { get; set; }
        public string? StringContent { get; set; }
        public Guid QuestionId { get; set; }
        public int OrderInList { get; set; }
    }
}
