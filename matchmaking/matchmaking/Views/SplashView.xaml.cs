using matchmaking.Domain;
using matchmaking.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;

namespace matchmaking.Views
{
    internal sealed partial class SplashView : Page
    {
        internal SplashViewModel? ViewModel { get; private set; }
        private CreateProfileViewModel? _createProfileViewModel;

        public SplashView()
        {
            InitializeComponent();
        }

        internal void SetViewModel(SplashViewModel viewModel, CreateProfileViewModel? createProfileViewModel = null)
        {
            ViewModel = viewModel;
            _createProfileViewModel = createProfileViewModel;
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            StartSplashTimer();
        }

        public void StartSplashTimer()
        {
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                ViewModel!.NavigateCommand.Execute(null);
            };
            timer.Start();
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.NextScreen))
            {
                NavigateTo(ViewModel!.NextScreen);
            }
        }

        private void NavigateTo(Screen screen)
        {
            switch (screen)
            {
                case Screen.AGE_BLOCK:
                    Frame.Navigate(typeof(AgeBlockView), ViewModel);
                    break;
                case Screen.ADMIN:
                    var adminViewModel = new AdminViewModel(
                        new Services.SupportTicketService(new Repositories.SupportTicketRepository(App.ConnectionString)),
                        new Services.ProfileService(new Repositories.ProfileRepository(App.ConnectionString), new Utils.MockUserUtil()));
                    Frame.Navigate(typeof(AdminView), adminViewModel);
                    break;
                case Screen.CREATE:
                    Frame.Navigate(typeof(CreateProfileView), _createProfileViewModel);
                    break;
                case Screen.DISCOVER:
                default:
                    var mainViewModel = new MainViewModel(ViewModel!.UserId, App.ConnectionString, false);
                    Frame.Navigate(typeof(MainView), mainViewModel);
                    break;
            }
        }
    }
}
