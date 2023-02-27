using FlashCards.Models.Quiz;

namespace FlashCards.Models.ViewModels
{
    public class CardSetQuizViewModel
    {
        public CardSet CardSet { get; set; }
        public List<FourOptionsQuizQuestion> FourOptionsQuestions { get; set; }
        public List<TrueFalseQuizQuestion> TrueFalseQuestions { get; set; }
        public List<BasicQuizQuestion> BasicQuizQuestions { get; set; }
        public int QuestionsCount =>
            FourOptionsQuestions.Count() +
            TrueFalseQuestions.Count() +
            BasicQuizQuestions.Count();
    }
}