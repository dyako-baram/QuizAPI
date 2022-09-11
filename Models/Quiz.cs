using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizAPI.Models
{
    public class Quiz
    {
        [Key]
        [Column("Id")]
        public int QuizId { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Genre { get; set; }
        [Column("Published")]
        public bool IsPublished { get; set; }
        [Required]
        public TimeSpan Duration { get; set; } 
        public string CoverImage { get; set; }

        public string QuizUserId { get; set; }
        public QuizUser QuizUser { get; set; }

        public List<Question> Questions { get; set; }
        public List<QuizAttempt> QuizAttempts { get; set; }

    }
}
