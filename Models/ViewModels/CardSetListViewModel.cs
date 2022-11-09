namespace FlashCards.Models.ViewModels
{
    public class CardSetListViewModel
    {
        public List<CardSet> CardSets { get; set; }
        public string? CategoryName { get; set; }
        public string? ActionName { get; set; } = "Sets";
        public string ListName { get; set; } = "Flashcard Sets";
    }
}
