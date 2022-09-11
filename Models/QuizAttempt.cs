using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizAPI.Models
{
    public class QuizAttempt
    {
        [Key]
        [Column("Id")]
        public int QuizAttemptId { get; set; }
        [Column("Start_date_time")]
        public DateTime StartDateTime { get; set; }
        [Column("End_date_time")]
        public DateTime EndDateTime { get; set; }
        public float SuccessRate { get; set; }
        public string QuizUserId { get; set; }
        public QuizUser QuizUser { get; set; }

        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
    }
}
