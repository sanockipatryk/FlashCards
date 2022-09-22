using System.ComponentModel.DataAnnotations;

namespace FlashCards.Models
{
    public class CardCategory
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public List<CardSubject> CardSubjects { get; set; }
    }
}
