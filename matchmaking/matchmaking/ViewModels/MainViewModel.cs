using matchmaking.Domain;
using matchmaking.Repositories;
using matchmaking.Services;
using matchmaking.Utils;

namespace matchmaking.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        private readonly int _userId;
        private readonly string _connectionString;

        public DiscoverViewModel DiscoverViewModel { get; }
        public NotificationsViewModel NotificationsViewModel { get; }
        public HotSeatViewModel HotSeatViewModel { get; }
        public EditProfileViewModel EditProfileViewModel { get; }

        public MainViewModel(int userId, string connectionString)
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
            var notificationService = new NotificationService(notificationRepo);
            var photoService = new PhotoService(photoRepo);
            var bidService = new BidService(bidRepo);
            var discoverService = new DiscoverService(profileRepo, interactionRepo, communityUtil, compatibilityUtil);
            var registerInteraction = new RegisterInteractionUseCase(interactionService, matchService, notificationService, profileRepo);

            DiscoverViewModel = new DiscoverViewModel(userId, discoverService, registerInteraction, true);
            NotificationsViewModel = new NotificationsViewModel(userId, notificationService);
            HotSeatViewModel = new HotSeatViewModel(userId, profileService, bidService, registerInteraction);
            EditProfileViewModel = new EditProfileViewModel(userId, profileService, photoService, questionaireUtil, interestUtil);
        }
    }
}
