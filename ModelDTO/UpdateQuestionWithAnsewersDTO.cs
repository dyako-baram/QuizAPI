namespace QuizAPI.ModelDTO
{
    public class UpdateQuestionWithAnsewersDTO
    {
        public string Description { get; set; }
        public List<UpdateAnswerDTO> Answers { get; set; }
    }
}
