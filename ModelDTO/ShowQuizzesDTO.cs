namespace QuizAPI.ModelDTO
{
    public class ShowQuizzesDTO
    {
        public int QuizId { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public bool IsPublished { get; set; }
        public TimeSpan Duration { get; set; }
        public string CoverImage { get; set; }
        public string UserName { get; set; }
        public List<QuestionDTO> Questions { get; set; }
        
    }
}
