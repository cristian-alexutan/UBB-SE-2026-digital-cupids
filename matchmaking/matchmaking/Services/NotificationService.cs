using System;
using System.Collections.Generic;
using System.Text;
using matchmaking.Repositories;
using matchmaking.Domain;

namespace matchmaking.Services
{
    internal class NotificationService
    {
        private readonly NotificationRepository _notificationRepository;

        public NotificationService(NotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public void AddNotification(Notification notification)
        {
            _notificationRepository.Add(notification);
        }

        public Notification FindById(int notificationId)
        {
            return _notificationRepository.FindById(notificationId);
        }

        public List<Notification> FindByRecipientId(int recipientId)
        {
            List<Notification> notifications = _notificationRepository.GetAll();
            List<Notification> result = new List<Notification>();
            foreach (Notification notification in notifications)
            {
                if (notification.RecipientId == recipientId)
                {
                    result.Add(notification);
                }
            }
            return result;
        }

        public Notification DeleteById(int notificationId)
        {
            return _notificationRepository.DeleteById(notificationId);
        }

        public List<Notification> DeleteByRecipientId(int recipientId)
        {
            List<Notification> notifications = FindByRecipientId(recipientId);
            List<Notification> result = new List<Notification>();
            foreach (Notification notification in notifications)
            {
                if (notification.RecipientId == recipientId)
                {
                    result.Add(notification);
                    _notificationRepository.DeleteById(notification.NotificationId);
                }
            }
            return result;
        }

        public Notification MarkReadById(int notificationId)
        {
            _notificationRepository.MarkRead(notificationId);
            Notification notification = _notificationRepository.FindById(notificationId);
            return notification;
        }

        public List<Notification> MarkReadByRecipientId(int recipientId)
        {
            List<Notification> notifications = FindByRecipientId(recipientId);
            List<Notification> result = new List<Notification>();
            foreach (Notification notification in notifications)
            {
                if (notification.RecipientId == recipientId)
                {
                    result.Add(notification);
                    _notificationRepository.MarkRead(notification.NotificationId);
                }
            }
            return result;
        }
    }
}
