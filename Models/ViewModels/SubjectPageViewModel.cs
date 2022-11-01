namespace FlashCards.Models.ViewModels
{
    public class SubjectPageViewModel
    {
        public CardSubject CardSubject { get; set; }
        public List<CardSubject> OtherSubjects { get; set; }
        public CardSetListViewModel CardSetListData { get; set; }
        public PaginationViewModel Pagination { get; set; }
        public CardSetFiltersViewModel Filters { get; set; }
    }
}
