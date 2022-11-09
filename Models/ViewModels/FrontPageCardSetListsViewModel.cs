namespace FlashCards.Models.ViewModels {
    public class FrontPageCardSetListsViewModel
    {
        public CardSetListViewModel MostRecentAccessedCardSets { get; set; }
        public CardSetListViewModel MostRecentUserCardSets { get; set; }
        public CardSetListViewModel MostRecentPublicCardSets { get; set; }
    }
}