using Microsoft.AspNetCore.Identity;

namespace FlashCards.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nickname { get; set; }
    }
}
