namespace FlashCards.Models.ViewModels
{
    public class CreateCardSetViewModel
    {
        public CardSet CardSet { get; set; }
        public int SelectedCardCategoryId { get; set; }
        public IEnumerable<CardCategory> CardCategories { get; set; }
    }
}
