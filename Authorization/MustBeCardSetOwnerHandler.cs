using FlashCards.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FlashCards.Authorization
{
    public class MustBeCardSetOwnerHandler : AuthorizationHandler<MustBeCardSetOwnerRequirement>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MustBeCardSetOwnerHandler(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustBeCardSetOwnerRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            var cardSetId = _httpContextAccessor.HttpContext.Request.RouteValues["id"];
            int cardSetIdAsInt = Convert.ToInt32(cardSetId);

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cardSet = await _dbContext.CardSets.FirstOrDefaultAsync(c => c.Id == cardSetIdAsInt);

            if (cardSet == null)
            {
                context.Fail();
                return;
            }
            if (cardSet.UserId != userId)
            {
                context.Fail();
                return;
            }
            context.Succeed(requirement);
        }
    }
}
