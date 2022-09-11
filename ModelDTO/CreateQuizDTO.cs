using QuizAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.ModelDTO
{
    public class CreateQuizDTO
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        public bool IsPublished { get; set; }
        [Required]
        public TimeSpan Duration { get; set; }
        [Required]
        public string CoverImage { get; set; }
        [Required]
        public List<QuestionDTO> Questions { get; set; }
    }
}
