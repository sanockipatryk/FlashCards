namespace FlashCards.Models.ViewModels
{
    public class SetsPageViewModel
    {
        public List<CardSet> CardSets { get; set; }
        public List<CardCategory> CardCategories { get; set; }
        public PaginationViewModel Pagination { get; set; }
        public CardSetFiltersViewModel Filters { get; set; }
    }
}
