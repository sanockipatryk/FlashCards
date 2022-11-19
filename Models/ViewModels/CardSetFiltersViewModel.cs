using FlashCards.Models.Types.Enums;

namespace FlashCards.Models.ViewModels
{
    public class CardSetFiltersViewModel
    {
        public string? Name { get; set; }
        public string? Author { get; set; }
        public SetsNumberOfCards? NumberOfCards { get; set; }
        public SetsSortBy? SortBy { get; set; } = SetsSortBy.Newest;
        public bool FiltersAplied
        {
            get
            {
                if (
                Name == null &&
                NumberOfCards == null &&
                Author == null &&
                (SortBy == null || SortBy == SetsSortBy.Newest))
                    return false;
                return true;
            }
        }
    }
}
