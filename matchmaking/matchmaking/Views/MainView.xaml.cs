using matchmaking.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace matchmaking.Views
{
    internal sealed partial class MainView : Page
    {
        internal MainViewModel? ViewModel { get; private set; }

        public MainView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = e.Parameter as MainViewModel;

            if (NavView.MenuItems.Count > 0)
            {
                NavView.SelectedItem = NavView.MenuItems[0];
            }
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item)
            {
                string tag = item.Tag?.ToString() ?? string.Empty;
                NavigateToTab(tag);
            }
        }

        private void NavigateToTab(string tag)
        {
            if (ViewModel == null) return;

            switch (tag)
            {
                case "Discover":
                    ContentFrame.Navigate(typeof(DiscoverView));
                    if (ContentFrame.Content is DiscoverView discoverView)
                    {
                        discoverView.SetViewModel(ViewModel.DiscoverViewModel);
                    }
                    break;

                case "Notifications":
                    ContentFrame.Navigate(typeof(NotificationsView));
                    if (ContentFrame.Content is NotificationsView notificationsView)
                    {
                        notificationsView.SetViewModel(ViewModel.NotificationsViewModel);
                    }
                    break;

                case "HotSeat":
                    ContentFrame.Navigate(typeof(HotSeatView));
                    if (ContentFrame.Content is HotSeatView hotSeatView)
                    {
                        hotSeatView.SetViewModel(ViewModel.HotSeatViewModel);
                    }
                    break;

                case "EditProfile":
                    ContentFrame.Navigate(typeof(EditProfileView), ViewModel.EditProfileViewModel);
                    break;
            }
        }
    }
}
