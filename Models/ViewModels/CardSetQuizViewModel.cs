using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCards.Models.ViewModels
{
    public class CardSetQuiz
    {
        public CardSet CardSet { get; set; }
        public List<QuizQuestion> QuizQuestions { get; set; }
    }
}
