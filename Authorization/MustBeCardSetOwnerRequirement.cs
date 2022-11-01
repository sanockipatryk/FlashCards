using Microsoft.AspNetCore.Authorization;

namespace FlashCards.Authorization
{
    public class MustBeCardSetOwnerRequirement : IAuthorizationRequirement
    {
        public MustBeCardSetOwnerRequirement()
        {
        }
    }
}
