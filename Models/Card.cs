using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCards.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        [ForeignKey(nameof(CardSet))]
        public int CardSetId { get; set; }
        public CardSet CardSet { get; set; }
    }
}
