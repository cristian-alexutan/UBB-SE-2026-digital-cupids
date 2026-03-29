using matchmaking.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace matchmaking.Views
{
    internal sealed partial class DiscoverView : Page
    {
        internal DiscoverViewModel? ViewModel { get; private set; }

        public DiscoverView()
        {
            InitializeComponent();
        }

        public void SetViewModel(DiscoverViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;
        }

        public Visibility Vis(bool condition)
        {
            return condition ? Visibility.Visible : Visibility.Collapsed;
        }

        public Visibility VisInverse(bool condition)
        {
            return condition ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                ViewModel?.CloseGuide();
            }
        }

        private void Guide_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel?.CloseGuide();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.OpenGuide();
        }
    }
}