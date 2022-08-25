namespace FlashCards.Models
{
    public class CardCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CardSubject> CardSubjects { get; set; }
    }
}
