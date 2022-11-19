using Microsoft.EntityFrameworkCore;
using FlashCards.Models;
using FlashCards.SSoT;
using System.Text;
using FlashCards.Models.ViewModels;
using FlashCards.Helpers;

namespace FlashCards.Data.Services
{
    public class NotificationService : INotificationsService
    {
        private readonly ApplicationDbContext _context;
        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        //TODO: change add nullable boolean if take 5 or if take all
        public async Task<UserNotificationsListViewModel> GetRecentNotificationsForUser(string userId, int amount, int? lastNotificationLoadedId)
        {
            var unreadCount = await _context.UserNotifications
                .CountAsync(n => n.RecipientId == userId && !n.IsRead);
            var allCount = await _context.UserNotifications
                .CountAsync(n => n.RecipientId == userId);

            UserNotification? lastNotificationLoaded = null;
            if (lastNotificationLoadedId != null)
            {
                lastNotificationLoaded = await _context.UserNotifications.FirstOrDefaultAsync(n => n.RecipientId == userId && n.Id == lastNotificationLoadedId);
            }

            var notifications = await _context.UserNotifications
                .Where(n => n.RecipientId == userId)
                .IfLoadingMore(lastNotificationLoaded)
                .OrderByDescending(n => n.DateSent)
                .Take(amount)
                .ToListAsync();

            return new UserNotificationsListViewModel
            {
                Notifications = notifications,
                UnreadNotificationsCount = unreadCount,
                AllNotificationsForUserCount = allCount,
            };
        }

        public async Task GetNotificationForUser(int notificationId, string userId)
        {
            var notification = await _context.UserNotifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.RecipientId == userId);
        }

        public async Task<bool> SendNotification(string recipientId, string title, string message)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == recipientId);

            if (user != null)
            {
                var notification = new UserNotification
                {
                    RecipientId = user.Id,
                    DateSent = DateTime.UtcNow,
                    IsRead = false,
                    Title = title,
                    Message = message
                };

                await _context.UserNotifications.AddAsync(notification);
                return true;
            }
            return false;
        }

        public async Task SetNotificationRead(int notificationId, string userId)
        {
            var notification = await _context.UserNotifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.RecipientId == userId && !n.IsRead);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ChangeNotificationStatus(int notificationId, string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                var notification = await _context.UserNotifications.FirstOrDefaultAsync(n => n.Id == notificationId);
                if (notification != null)
                {
                    if (!notification.IsRead)
                        notification.DateRead = DateTime.UtcNow;
                    else
                        notification.DateRead = null;
                    notification.IsRead = !notification.IsRead;

                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            return false;
        }

        public async Task<bool> DeleteNotification(int notificationId, string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                var notification = await _context.UserNotifications.FirstOrDefaultAsync(n => n.Id == notificationId);
                if (notification != null)
                {
                    _context.UserNotifications.Remove(notification);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}