using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FlashCards.Data.Services;
using FlashCards.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlashCards.Controllers
{
    [Route("[controller]")]
    public class NotificationsController : Controller
    {
        private readonly INotificationsService _notificationsService;

        public NotificationsController(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = UserManagerExtensions.GetUserId(User);

            var userNotifications = await _notificationsService.GetRecentNotificationsForUser(userId, 5, null);
            return View(userNotifications);
        }

        [Authorize]
        [HttpPost("LoadMoreNotifications")]
        public async Task<IActionResult> LoadMoreNotifications([FromBody] int lastNotificationLoadedId)
        {
            var userId = UserManagerExtensions.GetUserId(User);
            var userNotifications = await _notificationsService.GetRecentNotificationsForUser(userId, 5, lastNotificationLoadedId);
            return PartialView("_NotificationList", userNotifications);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("ReadOrUnreadNotification")]
        public async Task<IActionResult> ReadOrUnreadNotification([FromBody] int id)
        {
            var userId = UserManagerExtensions.GetUserId(User);
            var result = await _notificationsService.ChangeNotificationStatus(id, userId);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("DeleteNotification")]
        public async Task<IActionResult> DeleteNotification([FromBody] int id)
        {
            var userId = UserManagerExtensions.GetUserId(User);
            var result = await _notificationsService.DeleteNotification(id, userId);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

    }
}