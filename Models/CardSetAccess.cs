using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCards.Models
{
    public class CardSetAccess
    {
        public int Id { get; set; }
        public DateTime DateAccessed { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        
        [ForeignKey(nameof(CardSet))]
        public int CardSetId { get; set; }
        public CardSet CardSet { get; set; }
    }
}