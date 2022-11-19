namespace FlashCards.Models.ViewModels
{
    public class UserNotificationsListViewModel
    {
        public List<UserNotification> Notifications { get; set; }
        public int? UnreadNotificationsCount { get; set; }
        public int? AllNotificationsForUserCount { get; set; }
    }
}