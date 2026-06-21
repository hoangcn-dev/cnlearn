namespace HoangCN.LearnMS.DTOs
{
    public class BankQuestionWithAnswersDto : QuestionDto
    {
        public List<BankAnswerDto> Answers { get; set; } = [];
    }

    public class BankAnswerDto
    {
        public Guid QuestionAnswerId { get; set; }
        public string? StringContent { get; set; }
        public bool IsCorrectAnswer { get; set; }
    }
}
