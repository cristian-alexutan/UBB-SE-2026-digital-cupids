using System;
using System.Collections.Generic;
using System.Text;
using matchmaking.Domain;
using matchmaking.Services;
using System.ComponentModel;
using System.Linq;

namespace matchmaking.ViewModels
{
    internal class NotificationsViewModel : INotifyPropertyChanged
    {
        private readonly int _userid;
        private readonly NotificationService _notificationService;
        private List<Notification> _notifications;
        public event PropertyChangedEventHandler? PropertyChanged;

        public NotificationsViewModel(int id, NotificationService notificationService)
        {
            _userid = id;
            _notificationService = notificationService;
            _notifications = new List<Notification>();
        }
        public List<Notification> Notifications
        {
            get => _notifications;
            set
            {
                _notifications = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Notifications)));
            }
        }
        public void LoadNotifications()
        {
            Notifications = _notificationService.FindByRecipientId(_userid).OrderByDescending(n => n.CreatedAt).ToList();

        }
        public void MarkAsRead(int notificationId)
        {
            _notificationService.MarkReadById(notificationId);
            LoadNotifications();
        }
        public void MarkAllAsRead()
        {
            _notificationService.MarkReadByRecipientId(_userid);
            LoadNotifications();
        }
        public void Delete(int notificationId)
        {
            _notificationService.DeleteById(notificationId);
            LoadNotifications();
        }
        public void DeleteAll()
        {
            _notificationService.DeleteByRecipientId(_userid);
            LoadNotifications();
        }
        
    }
}
