using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCards.Models
{
    public class CardSet
    {
        public int Id { get; set; }
        [Required]
        [MinLength(5, ErrorMessage = "Name cannot be shorter than 5 characters.")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }
        [MinLength(5, ErrorMessage = "Description cannot be shorter than 5 characters.")]
        [MaxLength(300, ErrorMessage = "Description cannot be longer than 300 characters.")]
        public string? Description { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool IsPublic { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(User))]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [ForeignKey(nameof(CardSubject))]
        [Required, Range(1, Int32.MaxValue, ErrorMessage = "Please choose a subject.")]
        public int CardSubjectId { get; set; }
        public CardSubject? CardSubject { get; set; }

        public List<Card> Cards { get; set; }
        public List<CardSetReport>? CardSetReports { get; set; }
    }
}
