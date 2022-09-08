using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCards.Models
{
    public class CardSet
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool IsPublic { get; set; }
        
        [ForeignKey(nameof(User))]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [ForeignKey(nameof(CardSubject))]
        [Required, Range(1, Int32.MaxValue, ErrorMessage = "Please choose a subject.")]
        public int CardSubjectId { get; set; }
        public CardSubject? CardSubject { get; set; }

        public List<Card> Cards { get; set; }
    }
}
