namespace QuizAPI.ModelDTO
{
    public class QuestionDTO
    {
        public int QuestionId { get; set; }
        public string Description { get; set; }
        public List<AnswerDTO> Answers { get; set; }
    }
}
