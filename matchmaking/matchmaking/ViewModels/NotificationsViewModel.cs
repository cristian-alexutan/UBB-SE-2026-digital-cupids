using matchmaking.Domain;
using matchmaking.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace matchmaking.ViewModels
{
    internal class NotificationsViewModel : ObservableObject
    {
        private readonly int _userId;
        private readonly NotificationService _notificationService;
        private ObservableCollection<Notification> _notifications;

        public ICommand MarkAllAsReadCommand { get; }
        public ICommand DeleteAllCommand { get; }

        public NotificationsViewModel(int userId, NotificationService notificationService)
        {
            _userId = userId;
            _notificationService = notificationService;
            _notifications = new ObservableCollection<Notification>();

            MarkAllAsReadCommand = new RelayCommand(MarkAllAsRead);
            DeleteAllCommand = new RelayCommand(DeleteAll);
        }

        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set => SetProperty(ref _notifications, value);
        }

        public void LoadNotifications()
        {
            var list = _notificationService
                .FindByRecipientId(_userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

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
            _notificationService.MarkReadByRecipientId(_userId);
            LoadNotifications();
        }

        public void Delete(int notificationId)
        {
            _notificationService.DeleteById(notificationId);
            LoadNotifications();
        }

        public void DeleteAll()
        {
            _notificationService.DeleteByRecipientId(_userId);
            LoadNotifications();
        }
    }
}