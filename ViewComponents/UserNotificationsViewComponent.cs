using System.Security.Claims;
using FlashCards.Data.Services;
using FlashCards.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FlashCards.ViewComponents
{
    public class UserNotificationsViewComponent : ViewComponent
    {
        private readonly INotificationsService _service;
        public UserNotificationsViewComponent(INotificationsService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = UserManagerExtensions.GetUserId((ClaimsPrincipal)User);
            if (userId != null)
            {
                var notifications = await _service.GetRecentNotificationsForUser(userId, 5, null);
                return View(notifications);
            }
            return View();
        }
    }
}