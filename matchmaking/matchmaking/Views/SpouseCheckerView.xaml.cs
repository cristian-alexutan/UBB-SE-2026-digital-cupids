using matchmaking.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage.Pickers;

namespace matchmaking.Views
{
    public sealed partial class SpouseCheckerView : Page
    {
        internal SpouseCheckerViewModel ViewModel { get; private set; }

        public SpouseCheckerView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            ViewModel = (SpouseCheckerViewModel)e.Parameter;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CanSubmit())
            {
                ViewModel.Submit();
                Frame.GoBack();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Cancel();
            Frame.GoBack();
        }

        private async void BrowseMarriageCertificate_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".pdf");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file != null)
                ViewModel.MarriageCertificatePath = file.Path;
        }

        private async void BrowsePartnerPhoto_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file != null)
                ViewModel.PartnerPhotoPath = file.Path;
        }
    }
}