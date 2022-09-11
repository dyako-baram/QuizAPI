namespace QuizAPI.ModelDTO
{
    public class StartQuizDTO
    {
        public string UserId { get; set; }
        public int QuizId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public float SuccessRate { get; set; }

    }
}
