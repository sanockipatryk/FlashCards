namespace FlashCards.Models.ViewModels
{
    public class CardSetViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime DateUpdated { get; set; }
        public CardSubject CardSubject { get; set; }
        public int CardCount { get; set; }
    }
}
