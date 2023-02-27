namespace FlashCards.Models.Quiz
{
    public class BasicQuizQuestion
    {
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }
        public string? Answer { get; set; }

        public bool IsAnswerCorrect()
        {
            return CorrectAnswer.Equals(Answer);
        }
    }
}
