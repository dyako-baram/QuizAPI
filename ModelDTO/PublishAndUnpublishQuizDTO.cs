using System.ComponentModel.DataAnnotations;

namespace QuizAPI.ModelDTO
{
    public class PublishAndUnpublishQuizDTO
    {
        [Required]
        public bool IsPublished { get; set; }
    }
}
