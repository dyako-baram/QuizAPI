using System.ComponentModel.DataAnnotations;

namespace QuizAPI.ModelDTO
{
    public class UpdateQuestionDTO
    {
        [Required]
        public string Description { get; set; }
    }
}
