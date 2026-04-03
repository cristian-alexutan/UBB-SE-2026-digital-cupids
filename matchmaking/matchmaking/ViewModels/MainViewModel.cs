using matchmaking.Domain;
using matchmaking.Repositories;
using matchmaking.Services;
using matchmaking.Utils;
using Microsoft.UI.Dispatching;
using System;
using System.Collections.Generic;
using System.Linq;

namespace matchmaking.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        private readonly int _userId;
        private readonly string _connectionString;
        private readonly NotificationService _notificationService;

        private List<int> _knownNotificationIds = new();
        private DispatcherQueueTimer? _timer;

        private int _unreadCount;
        private bool _hasUnread;
        private string _popupTitle = string.Empty;
        private string _popupDescription = string.Empty;
        private bool _isPopupVisible;

        public DiscoverViewModel DiscoverViewModel { get; }
        public NotificationsViewModel NotificationsViewModel { get; }
        public HotSeatViewModel HotSeatViewModel { get; }
        public EditProfileViewModel EditProfileViewModel { get; }
        public int UserId => _userId;

        public int UnreadCount
        {
            get => _unreadCount;
            private set
            {
                SetProperty(ref _unreadCount, value);
                HasUnread = value > 0;
            }
        }

        public bool HasUnread
        {
            get => _hasUnread;
            private set => SetProperty(ref _hasUnread, value);
        }

        public string PopupTitle
        {
            get => _popupTitle;
            private set => SetProperty(ref _popupTitle, value);
        }

        public string PopupDescription
        {
            get => _popupDescription;
            private set => SetProperty(ref _popupDescription, value);
        }

        public bool IsPopupVisible
        {
            get => _isPopupVisible;
            private set => SetProperty(ref _isPopupVisible, value);
        }

        public MainViewModel(int userId, string connectionString, bool firstLoad = false)
        {
            _userId = userId;
            _connectionString = connectionString;

            var profileRepo = new ProfileRepository(connectionString);
            var interactionRepo = new InteractionRepository(connectionString);
            var matchRepo = new MatchRepository(connectionString);
            var notificationRepo = new NotificationRepository(connectionString);
            var photoRepo = new PhotoRepository(connectionString);
            var bidRepo = new BidRepository(connectionString);

            var mockUserUtil = new MockUserUtil();
            var communityUtil = new MockCommunityUtil();
            var compatibilityUtil = new CompatibilityUtil();
            var questionaireUtil = new QuestionaireUtil();
            var interestUtil = new InterestUtil();

            var profileService = new ProfileService(profileRepo, mockUserUtil);
            var interactionService = new InteractionService(interactionRepo);
            var matchService = new MatchService(matchRepo);
            _notificationService = new NotificationService(notificationRepo);
            var photoService = new PhotoService(photoRepo);
            var bidService = new BidService(bidRepo);
            var discoverService = new DiscoverService(profileRepo, interactionRepo, communityUtil, compatibilityUtil);
            var registerInteraction = new RegisterInteractionUseCase(interactionService, matchService, _notificationService, profileRepo);

            DiscoverViewModel = new DiscoverViewModel(userId, discoverService, registerInteraction, firstLoad);
            NotificationsViewModel = new NotificationsViewModel(userId, _notificationService);
            HotSeatViewModel = new HotSeatViewModel(userId, profileService, bidService, registerInteraction,interactionService);
            EditProfileViewModel = new EditProfileViewModel(userId, profileService, photoService, questionaireUtil, interestUtil);
        }

        public void InitializeNotifications()
        {
            var existing = _notificationService.FindByRecipientId(_userId);
            _knownNotificationIds = existing.Select(n => n.NotificationId).ToList();
            UpdateBadge();
        }

        public void StartPolling()
        {
            if (_timer != null) return;

            _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(10);
            _timer.Tick += (s, e) => CheckForNewNotifications();
            _timer.Start();
        }

        public void StopPolling()
        {
            _timer?.Stop();
            _timer = null;
        }

        public void UpdateBadge()
        {
            var notifications = _notificationService.FindByRecipientId(_userId);
            UnreadCount = notifications.Count(n => !n.IsRead);
        }

        private void CheckForNewNotifications()
        {
            var notifications = _notificationService.FindByRecipientId(_userId);
            var newNotifications = notifications
                .Where(n => !_knownNotificationIds.Contains(n.NotificationId))
                .ToList();

            foreach (Notification notification in newNotifications)
            {
                _knownNotificationIds.Add(notification.NotificationId);
                ShowPopup(notification);
            }

            UpdateBadge();
        }

        private async void ShowPopup(Notification notification)
        {
            PopupTitle = notification.Title;
            PopupDescription = notification.Description;
            IsPopupVisible = true;

            await System.Threading.Tasks.Task.Delay(4000);
            IsPopupVisible = false;
        }
    }
}
