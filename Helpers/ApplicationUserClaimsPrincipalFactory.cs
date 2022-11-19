using FlashCards.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace FlashCards.Helpers
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
                : base(userManager, optionsAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var roles = await base.UserManager.GetRolesAsync(user);

            
            identity.AddClaim(new Claim("Nickname", user.Nickname ?? ""));
            identity.AddClaim(new Claim(ClaimTypes.Role, roles[0]));

            return identity;
        }
    }
}
