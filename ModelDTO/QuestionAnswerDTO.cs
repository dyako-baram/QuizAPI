using System.ComponentModel.DataAnnotations;

namespace QuizAPI.ModelDTO
{
    public class QuestionAnswerDTO
    {
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CurrentTime { get; set; } = DateTime.UtcNow;
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TimeTaken { get; set; }
        [Required]
        public int[] ChosenAnswerIds { get; set; }
    }
}
