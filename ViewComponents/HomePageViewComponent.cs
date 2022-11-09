using FlashCards.Data.Services;
using FlashCards.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlashCards.ViewComponents
{
    public class HomePageViewComponent : ViewComponent
    {
        private readonly ICardsService _service;
        public HomePageViewComponent(ICardsService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string? userId = UserManagerExtensions.GetUserId((ClaimsPrincipal) User);
            var categories = await _service.GetFrontPageCardSetsAsync(userId);
            return View(categories);
        }
    }
}
