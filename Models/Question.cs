using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizAPI.Models
{
    public class Question
    {
        [Key]
        [Column("Id")]
        public int QuestionId { get; set; }
        [Required]
        public string Description { get; set; }

        public int QuizId { get; set; }

        public Quiz Quiz { get; set; }

        public List<Answer> Answers { get; set; }

    }
}
