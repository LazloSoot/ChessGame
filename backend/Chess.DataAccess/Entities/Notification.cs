using Chess.DataAccess.Helpers;

namespace Chess.DataAccess.Entities
{
    public class Notification: Entity
    {
        public string Title { get; set; }
        public string BodyText { get; set; }
        public NotificationStatus Status { get; set; }
        public NotificationType Type { get; set; }
    }
}
