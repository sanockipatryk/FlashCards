namespace FlashCards.Models.Quiz
{
    public class TrueFalseQuizQuestion : BasicQuizQuestion
    {
        public bool CorrectAnswer { get; set; }
        public string PossibleAnswer { get; set; }
    }
}
