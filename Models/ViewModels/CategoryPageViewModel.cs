namespace FlashCards.Models.ViewModels
{
    public class CategoryPageViewModel
    {
        public CardCategory CardCategory { get; set; }
        public List<CardCategory> OtherCategories { get; set; }
        public List<CardSet> CardSets { get; set; }
        public PaginationViewModel Pagination { get; set; }
        public bool PublicSets { get; set; }
        public CardSetFiltersViewModel Filters { get; set; }
    }
}
