using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlashCards.Models
{
    public class UserSetsData
    {
        [DisplayName("Your sets")]
       public int TotalSetCount {get; set; }
        [DisplayName("Your public sets")]
       public int PublicSetCount {get; set; }
        [DisplayName("Your private sets")]
       public int PrivateSetCount {get; set; }
        [DisplayName("Cards in your sets")]
       public int TotalCardsCount {get; set; }
    }
}