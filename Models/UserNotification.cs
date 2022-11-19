using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCards.Models
{
    public class UserNotification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime DateSent { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }

        [ForeignKey(nameof(User))]
        public string RecipientId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
