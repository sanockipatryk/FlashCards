using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCards.Models
{
    public class Card
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Question cannot be empty.")]
        public string Question { get; set; }
        [Required(ErrorMessage = "Answer cannot be empty.")]
        public string Answer { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        [ForeignKey(nameof(CardSet))]
        public int CardSetId { get; set; }
        public CardSet? CardSet { get; set; }
    }
}
