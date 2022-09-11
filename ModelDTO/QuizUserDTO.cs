namespace QuizAPI.ModelDTO
{
    public class QuizUserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime Age { get; set; }
        public string Country { get; set; }
        public IList<string> Roles { get; set; }
    }
}
