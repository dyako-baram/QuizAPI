using System.ComponentModel.DataAnnotations;

namespace QuizAPI.ModelDTO
{
    public class UpdateDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Age { get; set; }
        [Required]
        public string Country { get; set; }
    }
}
