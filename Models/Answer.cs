using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizAPI.Models
{
    public class Answer
    {
        [Key]
        [Column("Id")]
        public int AnswerId { get; set; }
        [Required]
        public string Description { get; set; }
        [Column("is_correct_answer")]
        public bool IsCorrectAnswer { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }



    }
}
