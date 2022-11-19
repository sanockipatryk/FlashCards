using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCards.Models
{
    public class CardSetFavorite
    {
        public int Id { get; set; }

        [ForeignKey(nameof(CardSet))]
        public int CardSetId { get; set; }
        public CardSet CardSet { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
