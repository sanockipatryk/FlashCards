using FlashCards.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FlashCards.Helpers
{
    public static class UserManagerExtensions
    {
        public static Task<ApplicationUser> FindByNicknameAsync(this UserManager<ApplicationUser> userManager, string nickName)
        {
            return userManager?.Users?.FirstOrDefaultAsync(um => um.Nickname == nickName);
        }
    }
}
