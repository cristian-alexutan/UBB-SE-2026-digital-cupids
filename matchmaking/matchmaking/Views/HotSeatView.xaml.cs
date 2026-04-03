using matchmaking.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace matchmaking.Views
{
    internal sealed partial class HotSeatView : Page
    {
        internal HotSeatViewModel? ViewModel { get; private set; }

        public HotSeatView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is HotSeatViewModel viewModel)
            {
                SetViewModel(viewModel);
            }
        }

        public void SetViewModel(HotSeatViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;
            ViewModel.LoadHotSeat();
        }

        public Visibility Vis(bool condition)
            => condition ? Visibility.Visible : Visibility.Collapsed;

        public Visibility VisInverse(bool condition)
            => condition ? Visibility.Collapsed : Visibility.Visible;

        private void PlaceBid_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null) return;
            ViewModel.BidInput = (int)BidInputBox.Value;
            ViewModel.PlaceBid();
            BidInputBox.Value = BidInputBox.Minimum;
        }

        private void BoostProfile_Click(object sender, RoutedEventArgs e)
            => ViewModel?.BoostProfile();

        private void LikeHotSeat_Click(object sender, RoutedEventArgs e)
            => ViewModel?.LikeHotSeat();

        private void SuperLikeHotSeat_Click(object sender, RoutedEventArgs e)
            => ViewModel?.SuperLikeHotSeat();

        private void NextPhoto_Click(object sender, RoutedEventArgs e)
            => ViewModel?.NextPhoto();

        private void PreviousPhoto_Click(object sender, RoutedEventArgs e)
            => ViewModel?.PreviousPhoto();
    }
}
