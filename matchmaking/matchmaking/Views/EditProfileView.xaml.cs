using matchmaking.Domain;
using matchmaking.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace matchmaking.Views
{

    public sealed partial class EditProfileView : Page
    {
        internal EditProfileViewModel? ViewModel { get; private set; }
        public EditProfileView()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = e.Parameter as EditProfileViewModel;
            this.DataContext = ViewModel;

            PhotosListView.Items.VectorChanged += PhotoItems_VectorChanged;
            BioBox.TextChanged += BioBox_TextChanged;
            BioBox.BeforeTextChanging += BioBox_BeforeTextChanging;

            RenderPhotos();
            RenderInterests();
            RenderPreferredGenders();
            UpdateInterestsHeader();
            UpdateBioHeader();
            UpdateArchivedBanner();
            UpdateQuestionnaireButton(); 
        }

        private void RenderPhotos()
        {
            PhotosListView.Items.Clear();

            foreach (Photo photo in ViewModel.Photos)
            {
                Grid slot = new Grid();
                slot.Width = 120;
                slot.Height = 120;
                slot.Margin = new Thickness(0, 0, 8, 0);
                slot.Tag = photo.PhotoId;

                Image img = new Image();
                img.Width = 120;
                img.Height = 120;
                img.Stretch = Microsoft.UI.Xaml.Media.Stretch.UniformToFill;
                string location = photo.Location;
                img.Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
                    new Uri("file:///" + photo.Location.Replace("\\", "/")));
                Button deleteBtn = new Button();
                deleteBtn.Content = "✕";
                deleteBtn.Width = 28;
                deleteBtn.Height = 28;
                deleteBtn.Padding = new Thickness(0);
                deleteBtn.CornerRadius = new CornerRadius(14);
                deleteBtn.Background = new SolidColorBrush(Microsoft.UI.Colors.Red);
                deleteBtn.Foreground = new SolidColorBrush(Microsoft.UI.Colors.White);
                deleteBtn.HorizontalAlignment = HorizontalAlignment.Right;
                deleteBtn.VerticalAlignment = VerticalAlignment.Top;
                deleteBtn.Margin = new Thickness(0, -8, -8, 0);
                deleteBtn.IsEnabled = ViewModel.CanRemovePhoto();

                int photoId = photo.PhotoId;
                deleteBtn.Click += (s, e) =>
                {
                    if (ViewModel.CanRemovePhoto())
                    {
                        ViewModel.RemovePhoto(photoId);
                        RenderPhotos();
                    }
                };

                slot.Children.Add(img);
                slot.Children.Add(deleteBtn);
                PhotosListView.Items.Add(slot);
            }

            if (ViewModel.Photos.Count < 6)
            {
                Border addSlot = new Border();
                addSlot.Width = 120;
                addSlot.Height = 120;
                addSlot.BorderBrush = new SolidColorBrush(Microsoft.UI.Colors.LightGray);
                addSlot.BorderThickness = new Thickness(1);
                addSlot.CornerRadius = new CornerRadius(8);
                addSlot.Margin = new Thickness(0, 0, 8, 0);

                Button addBtn = new Button();
                addBtn.Content = "+";
                addBtn.FontSize = 24;
                addBtn.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Gray);
                addBtn.Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
                addBtn.HorizontalAlignment = HorizontalAlignment.Center;
                addBtn.VerticalAlignment = VerticalAlignment.Center;
                addBtn.Click += AddPhoto_Click;

                addSlot.Child = addBtn;
                PhotosListView.Items.Add(addSlot);
            }

            int emptySlots = 6 - ViewModel.Photos.Count - 1;
            for (int i = 0; i < emptySlots; i++)
            {
                Border emptySlot = new Border();
                emptySlot.Width = 120;
                emptySlot.Height = 120;
                emptySlot.BorderBrush = new SolidColorBrush(Microsoft.UI.Colors.LightGray);
                emptySlot.BorderThickness = new Thickness(1);
                emptySlot.CornerRadius = new CornerRadius(8);
                emptySlot.Margin = new Thickness(0, 0, 8, 0);
                PhotosListView.Items.Add(emptySlot);
            }

        }

        private void PhotoItems_VectorChanged(
            Windows.Foundation.Collections.IObservableVector<object> sender,
            Windows.Foundation.Collections.IVectorChangedEventArgs e)
        {
            List<int> newOrder = PhotosListView.Items
                .OfType<Grid>()
                .Where(g => g.Tag is int)
                .Select(g => (int)g.Tag)
                .ToList();

            if (newOrder.Count == ViewModel.Photos.Count)
            {
                ViewModel.ReorderPhotos(newOrder);
            }
        }

        private async void AddPhoto_Click(object sender, RoutedEventArgs e)
        {

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(
                (Application.Current as App)._window);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                Photo newPhoto = new Photo(ViewModel._userId, file.Path, ViewModel.Photos.Count);
                ViewModel.AddPhoto(newPhoto);
                RenderPhotos();
            }
        }

        private void RenderPreferredGenders()
        {
            PreferredGendersPanel.Children.Clear();
            Gender[] genderValues = { Gender.MALE, Gender.FEMALE, Gender.NON_BINARY, Gender.OTHER };
            List<string> genders = ViewModel.GenderOptions;
            for (int i = 0; i < genders.Count; i++)
            {
                Gender g = genderValues[i];
                bool isSelected = ViewModel.PreferredGenders.Contains(g);

                Button btn = new Button();
                btn.Content = genders[i];
                btn.CornerRadius = new CornerRadius(20);
                btn.Padding = new Thickness(16, 8, 16, 8);

                if (isSelected)
                {
                    btn.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
                    btn.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
                }
                else
                {
                    btn.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                    btn.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
                }

                btn.Click += (s, e) =>
                {
                    if (ViewModel.PreferredGenders.Contains(g))
                        ViewModel.PreferredGenders.Remove(g);
                    else
                        ViewModel.PreferredGenders.Add(g);
                    RenderPreferredGenders();
                };

                PreferredGendersPanel.Children.Add(btn);
            }
        }

        private void RenderInterests()
        {
            InterestsPanel.Children.Clear();

            foreach (string interest in ViewModel.AllInterests)
            {
                bool isSelected = ViewModel.Interests.Contains(interest);

                Button btn = new Button();
                btn.Content = interest;
                btn.CornerRadius = new CornerRadius(20);
                btn.Padding = new Thickness(16, 8, 16, 8);
                btn.Margin = new Thickness(0, 0, 8, 8);

                if (isSelected)
                {
                    btn.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
                    btn.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
                    btn.IsEnabled = ViewModel.CanRemoveInterest();
                }
                else
                {
                    btn.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                    btn.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
                    btn.IsEnabled = ViewModel.CanAddInterest();
                }

                btn.Click += (s, e) =>
                {
                    if (isSelected && ViewModel.CanRemoveInterest(interest))
                        ViewModel.RemoveInterest(interest);
                    else if (!isSelected && ViewModel.CanAddInterest())
                        ViewModel.AddInterest(interest);

                    UpdateInterestsHeader();
                    RenderInterests();
                };

                InterestsPanel.Children.Add(btn);
            }
        }
        private void UpdateInterestsHeader()
        {
            InterestsHeader.Text = $"Interests ({ViewModel.Interests.Count}/15)";
        }

        private void UpdateBioHeader()
        {
            BioHeader.Text = $"Bio ({BioBox.Text.Length}/250)";
        }

        private void BioBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (sender.Text.Length >= 250)
            {
                args.Cancel = true;
            }
        }

        private void BioBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateBioHeader();
        }

        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Save Changes";
            dialog.Content = "Are you sure you want to save your changes?";
            dialog.PrimaryButtonText = "Save";
            dialog.CloseButtonText = "Cancel";
            dialog.XamlRoot = this.XamlRoot;

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.SaveChanges();
            }
        }

        private async void DiscardChanges_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Discard Changes";
            dialog.Content = "Are you sure you want to discard all changes? This cannot be undone.";
            dialog.PrimaryButtonText = "Discard";
            dialog.CloseButtonText = "Cancel";
            dialog.XamlRoot = this.XamlRoot;

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.DiscardChanges();
                RenderPhotos();
                RenderInterests();
                RenderPreferredGenders();
                UpdateInterestsHeader();
                UpdateBioHeader();
            }
        }

        private void ArchiveProfile_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ArchiveProfile();
            UpdateArchivedBanner();
        }
        private void UnarchiveProfile_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.UnarchiveProfile();
            UpdateArchivedBanner();
        }

        private async void DeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Delete Profile";
            dialog.Content = "Are you sure you want to delete your profile? This cannot be undone.";
            dialog.PrimaryButtonText = "Delete";
            dialog.CloseButtonText = "Cancel";
            dialog.XamlRoot = this.XamlRoot;

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.DeleteProfile();
            }
        }

        private void UpdateArchivedBanner()
        {
            ArchivedBanner.Visibility = ViewModel.IsArchived
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void QuestionnaireButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel!.PrepareQuestionnaire();
            Frame.Navigate(typeof(QuestionnaireView), ViewModel);
        }
        private void UpdateQuestionnaireButton()
        {
            QuestionnaireButton.Visibility = ViewModel.HasLoverType
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}
