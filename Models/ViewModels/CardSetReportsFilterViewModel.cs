using FlashCards.Models.Types.Enums;

namespace FlashCards.Models.ViewModels
{
    public class CardSetReportsFilterViewModel
    {
        public int? CardSetId { get; set; }
        public ReportsShow? Show { get; set; } = ReportsShow.Unevaluated;
        public ReportsSortBy? SortBy { get; set; } = ReportsSortBy.Newest;
        public ReportCause? Cause { get; set; }
        public bool FiltersAplied
        {
            get
            {
                if (
                CardSetId == null &&
                (Show == null || Show == ReportsShow.Unevaluated) &&
                (SortBy == null || SortBy == ReportsSortBy.Newest) &&
                Cause == null)
                    return false;
                return true;
            }
        }
    }
}