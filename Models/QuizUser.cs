using Microsoft.AspNetCore.Identity;

namespace QuizAPI.Models
{
    public class QuizUser : IdentityUser
    {
        public string Country { get; set; }
        public DateTime Age { get; set; }
        public List<Quiz> Quizzes { get; set; }
        public List<QuizAttempt> QuizAttempts { get; set; }
    }
}
