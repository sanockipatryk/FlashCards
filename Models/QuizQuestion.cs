namespace FlashCards.Models
{
    public class QuizQuestion
    {
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }
        public List<string>? PossibleAnswers { get; set; }
    }
}
