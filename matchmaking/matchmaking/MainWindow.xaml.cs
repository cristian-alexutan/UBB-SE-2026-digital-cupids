using matchmaking.Repositories;
using matchmaking.Services;
using matchmaking.Utils;
using matchmaking.ViewModels;
using matchmaking.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.System;

namespace matchmaking
{
    public sealed partial class MainWindow : Window
    {
        private static readonly int UserId = App.UserID;
        private const string SecretCode = "DOUBT112";
        private string _typedBuffer = string.Empty;

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

            if (Content is UIElement rootElement)
            {
                rootElement.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(OnGlobalKeyDown), true);
            }
            RootFrame.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(OnGlobalKeyDown), true);
        }

        private void OnGlobalKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.OriginalSource is DependencyObject source && IsTextInputSource(source))
            {
                return;
            }

            if (!TryMapKeyToSecretChar(e.Key, out char typedChar))
            {
                return;
            }

            _typedBuffer += typedChar;
            if (_typedBuffer.Length > SecretCode.Length)
            {
                _typedBuffer = _typedBuffer[^SecretCode.Length..];
            }

            if (_typedBuffer.EndsWith(SecretCode, System.StringComparison.OrdinalIgnoreCase))
            {
                _typedBuffer = string.Empty;
                OpenSpouseCheckerView();
            }
        }

        private static bool TryMapKeyToSecretChar(VirtualKey key, out char result)
        {
            if (key >= VirtualKey.A && key <= VirtualKey.Z)
            {
                result = (char)('A' + (key - VirtualKey.A));
                return true;
            }

            if (key >= VirtualKey.Number0 && key <= VirtualKey.Number9)
            {
                result = (char)('0' + (key - VirtualKey.Number0));
                return true;
            }

            if (key >= VirtualKey.NumberPad0 && key <= VirtualKey.NumberPad9)
            {
                result = (char)('0' + (key - VirtualKey.NumberPad0));
                return true;
            }

            result = default;
            return false;
        }

        private static bool IsTextInputSource(DependencyObject source)
        {
            DependencyObject? current = source;
            while (current != null)
            {
                if (current is TextBox || current is PasswordBox || current is RichEditBox)
                {
                    return true;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return false;
        }

        private void OpenSpouseCheckerView()
        {
            SupportTicketRepository supportTicketRepository = new SupportTicketRepository(App.ConnectionString);
            SupportTicketService supportTicketService = new SupportTicketService(supportTicketRepository);
            SpouseCheckerViewModel spouseCheckerViewModel = new SpouseCheckerViewModel(supportTicketService);

            RootFrame.Navigate(typeof(SpouseCheckerView), spouseCheckerViewModel);
        }
    }
}
