using matchmaking.Domain;
using matchmaking.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace matchmaking.ViewModels
{
    internal class NotificationsViewModel : INotifyPropertyChanged
    {
        private readonly int _userid;
        private readonly NotificationService _notificationService;
        private ObservableCollection<Notification> _notifications;
        public event PropertyChangedEventHandler? PropertyChanged;

        public NotificationsViewModel(int id, NotificationService notificationService)
        {
            _userid = id;
            _notificationService = notificationService;
            _notifications = new ObservableCollection<Notification>();
        }
        public ObservableCollection<Notification> Notifications
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
            var list = _notificationService.FindByRecipientId(_userid).OrderByDescending(n => n.CreatedAt).ToList();
            _notifications.Clear();
            foreach (Notification n in list)
            {
                _notifications.Add(n);
            }
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
