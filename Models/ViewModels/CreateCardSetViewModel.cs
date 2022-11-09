using FlashCards.Enums;
using System.ComponentModel.DataAnnotations;

namespace FlashCards.Models.ViewModels
{
    public class CreateCardSetViewModel
    {
        public CardSet CardSet { get; set; }
        [Required, Range(1, Int32.MaxValue, ErrorMessage = "Please choose a category.")]
        public int SelectedCardCategoryId { get; set; }
        public IEnumerable<CardCategory>? CardCategories { get; set; }
        public string? CardSubjectsListJson {get; set; }
        public int AddManyCards { get; set; }
        public CreateSetActionType ActionType { get; set; }
    }
}
