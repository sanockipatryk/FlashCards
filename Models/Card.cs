using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCards.Models
{
    public class Card
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Question cannot be empty.")]
        [MinLength(1, ErrorMessage = "Question cannot be shorter than 1 character.")]
        [MaxLength(300, ErrorMessage = "Question cannot be longer than 300 characters.")]
        public string Question { get; set; }
        [Required(ErrorMessage = "Answer cannot be empty.")]
        [MinLength(1, ErrorMessage = "Answer cannot be shorter than 1 character.")]
        [MaxLength(300, ErrorMessage = "Answer cannot be longer than 300 characters.")]
        public string Answer { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        [ForeignKey(nameof(CardSet))]
        public int CardSetId { get; set; }
        public CardSet? CardSet { get; set; }
    }
}
