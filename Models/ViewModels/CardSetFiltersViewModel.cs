namespace FlashCards.Models.ViewModels
{
    public class CardSetFiltersViewModel
    {
        public string? Name { get; set; }
        public string? Author { get; set; }
        public string? NumberOfCards { get; set; }
        public string? SortBy { get; set; }
        public bool FiltersAplied {get { 
            if(
            Name == null && 
            NumberOfCards == null && 
            Author == null && 
            (SortBy == null || SortBy == "newest"))
                return false;
            return true;
        }}
    }
}
