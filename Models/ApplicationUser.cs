using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FlashCards.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(15, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
        public string Nickname { get; set; }
    }
}
