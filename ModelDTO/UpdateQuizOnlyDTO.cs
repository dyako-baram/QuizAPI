using System.ComponentModel.DataAnnotations;

namespace QuizAPI.ModelDTO
{
    public class UpdateQuizOnlyDTO
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
    }
}
