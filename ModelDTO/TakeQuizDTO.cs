namespace QuizAPI.ModelDTO
{
    public class TakeQuizDTO
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public float SuccessRate { get; set; }
        public string QuizUserId { get; set; }
        public int QuizId { get; set; }
    }
}
