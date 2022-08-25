using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCards.Models
{
    public class CardSet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsPublic { get; set; }
        
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey(nameof(CardSubject))]
        public int CardSubjectId { get; set; }
        public CardSubject CardSubject { get; set; }

        public List<Card> Cards { get; set; }
    }
}
