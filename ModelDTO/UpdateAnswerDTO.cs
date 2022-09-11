using System.ComponentModel.DataAnnotations;

namespace QuizAPI.ModelDTO
{
    public class UpdateAnswerDTO
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public bool IsCorrectAnswer { get; set; }
    }
}
