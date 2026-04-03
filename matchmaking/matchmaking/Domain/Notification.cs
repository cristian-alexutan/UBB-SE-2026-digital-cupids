using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matchmaking.Domain
{
    internal enum NotificationType
    {
        SUPER_LIKE,
        MATCH
    }
    internal class Notification
    {
        public int NotificationId { get; }
        public int RecipientId { get; }
        public int FromId { get; }
        public NotificationType Type { get; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; }
        public string Title { get; }
        public string Description { get; }

        public Notification(int recipientId, int fromId, NotificationType notificationType, string title, string description)
        {
            RecipientId = recipientId;
            FromId = fromId;
            Type = notificationType;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
            Title = title;
            Description = description;
        }

        public Notification(int notificationId, int recipientId, int fromId, NotificationType notificationType, bool isRead, DateTime createdAt, string title, string description)
        {
            NotificationId = notificationId;
            RecipientId = recipientId;
            FromId = fromId;
            Type = notificationType;
            IsRead = isRead;
            CreatedAt = createdAt;
            Title = title;
            Description = description;
        }
    }
}
