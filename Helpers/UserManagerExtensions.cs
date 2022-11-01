using FlashCards.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FlashCards.Helpers
{
    public static class UserManagerExtensions
    {
        public static Task<ApplicationUser> FindByNicknameAsync(this UserManager<ApplicationUser> userManager, string nickName)
        {
            return userManager?.Users?.FirstOrDefaultAsync(um => um.Nickname == nickName);
        }

        public static string? GetUserId(ClaimsPrincipal user)
        {
            string? userId = null;
            if (user.Identity.IsAuthenticated != null)
                userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            return userId;
        }
    }
}
