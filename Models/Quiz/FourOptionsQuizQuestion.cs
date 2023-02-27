namespace FlashCards.Models.Quiz
{
    public class FourOptionsQuizQuestion : BasicQuizQuestion
    {
        public string CorrectAnswer { get; set; }
        public List<string>? PossibleAnswers { get; set; }
    }
}
