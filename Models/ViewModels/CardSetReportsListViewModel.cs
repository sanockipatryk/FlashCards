namespace FlashCards.Models.ViewModels
{
    public class CardSetReportsListViewModel
    {
        public List<CardSetReport> Reports { get; set; }
        public CardSetReportsFilterViewModel Filters { get; set; }
        public PaginationViewModel Pagination { get; set; }
    }
}