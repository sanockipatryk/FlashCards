using FlashCards.Models.ViewModels;

namespace FlashCards.Data.Services
{
    public interface INotificationsService
    {
        Task<UserNotificationsListViewModel> GetRecentNotificationsForUser(string userId, int amount, int? lastNotificationLoadedId);
        Task GetNotificationForUser(int notificationId, string userId);
        Task<bool> SendNotification(string recipientId, string title, string message);
        Task SetNotificationRead(int notificationId, string userId);
        Task<bool> ChangeNotificationStatus(int notificationId, string userId);
        Task<bool> DeleteNotification(int notificationId, string userId);

    }
}