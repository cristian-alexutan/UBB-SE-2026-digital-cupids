using matchmaking.Repositories;
using matchmaking.Services;
using matchmaking.Utils;
using matchmaking.ViewModels;
using matchmaking.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace matchmaking
{
    public sealed partial class MainWindow : Window
    {
        private const int UserId = 40;

        public MainWindow()
        {
            InitializeComponent();

            string connectionString = App.ConnectionString;

            var profileRepo = new ProfileRepository(connectionString);
            var adminRepo = new DatingAdminRepository(connectionString);
            var mockUserUtil = new MockUserUtil();

            var profileService = new ProfileService(profileRepo, mockUserUtil);
            var adminService = new DatingAdminService(adminRepo);

            var splashViewModel = new SplashViewModel(UserId, mockUserUtil, profileService, adminService);
            var createProfileViewModel = new CreateProfileViewModel(UserId, profileService, mockUserUtil);

            RootFrame.Navigate(typeof(SplashView));
            if (RootFrame.Content is SplashView splashView)
            {
                splashView.SetViewModel(splashViewModel, createProfileViewModel);
            }
        }
    }
}
