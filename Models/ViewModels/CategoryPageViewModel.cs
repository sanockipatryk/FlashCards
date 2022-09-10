namespace FlashCards.Models.ViewModels
{
    public class CategoryPageViewModel
    {
        public CardCategory CardCategory { get; set; }
        public List<CardSet> CardSets { get; set; }
        public PaginationViewModel Pagination { get; set; }
    }
}
