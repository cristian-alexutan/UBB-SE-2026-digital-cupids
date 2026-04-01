using matchmaking.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Threading.Tasks;

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

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                if (e.Key == Windows.System.VirtualKey.Escape)
                {
                    ViewModel?.CloseGuide();
                }
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync(ex.Message);
            }
        }

        private void Guide_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                ViewModel?.CloseGuide();
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync(ex.Message);
            }
        }

        private void PreviousPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel?.PreviousPhoto();
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync(ex.Message);
            }
        }

        private void NextPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel?.NextPhoto();
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync(ex.Message);
            }
        }

        private void PassButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel?.PassCurrent();
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync(ex.Message);
            }
        }

        private void SuperLikeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel?.SuperLikeCurrent();
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync(ex.Message);
            }
        }

        private void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel?.LikeCurrent();
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync(ex.Message);
            }
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel?.OpenGuide();
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync(ex.Message);
            }
        }

        private void CloseMatchPopupButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel?.CloseMatchPopup();
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync(ex.Message);
            }
        }

        private async Task ShowErrorAsync(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();
        }
    }
}