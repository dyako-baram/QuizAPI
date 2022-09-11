namespace QuizAPI.ModelDTO
{
    public class UserQuizResult
    {
        public string QuizDescription { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public float QuizSuccessRate { get; set; }
    }
}
