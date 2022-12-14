using System.ComponentModel.DataAnnotations;

namespace FlashCards.Models.ViewModels
{
    public class EditCardSetViewModel
    {
        public CardSet CardSet { get; set; }
        [Required, Range(1, Int32.MaxValue, ErrorMessage = "Please choose a category.")]
        public int SelectedCardCategoryId { get; set; }
        public IEnumerable<CardCategory>? CardCategories { get; set; }
        public int AddManyCards { get; set; }
    }
}
