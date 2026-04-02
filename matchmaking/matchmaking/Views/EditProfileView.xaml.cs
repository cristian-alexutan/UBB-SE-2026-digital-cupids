using matchmaking.Domain;
using matchmaking.Services;
using matchmaking.Utils;
using matchmaking.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private bool _isRenderingPhotos = false;

        private const double MapLatNorth = 48.265;
        private const double MapLatSouth = 43.618;
        private const double MapLonWest = 20.261;
        private const double MapLonEast = 29.757;
        private const double ImgMarginTop = 0.03;
        private const double ImgMarginBottom = 0.93;
        private const double ImgMarginLeft = 0.04;
        private const double ImgMarginRight = 0.96;

        private (double lat, double lon)? _userCityCoords = null;

        public EditProfileView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = e.Parameter as EditProfileViewModel;
            this.DataContext = ViewModel;

            PhotosListView.Items.VectorChanged -= PhotoItems_VectorChanged;
            PhotosListView.Items.VectorChanged += PhotoItems_VectorChanged;

            BioBox.TextChanged -= BioBox_TextChanged;
            BioBox.TextChanged += BioBox_TextChanged;

            BioBox.BeforeTextChanging -= BioBox_BeforeTextChanging;
            BioBox.BeforeTextChanging += BioBox_BeforeTextChanging;

            DistanceSlider.ValueChanged -= DistanceSlider_ValueChanged;
            DistanceSlider.ValueChanged += DistanceSlider_ValueChanged;

            LocationComboBox.SelectionChanged -= HandleLocationChanged;
            LocationComboBox.SelectionChanged += HandleLocationChanged;

            LoadLocations();
            MapCanvas.SizeChanged -= MapCanvas_SizeChanged;
            MapCanvas.SizeChanged += MapCanvas_SizeChanged;

            LoadCityCoordinates();
            RefreshAllUI();
        }

        private Grid CreatePhotoSlot(Photo photo)
        {
            Grid slot = new Grid() { Width = 120, Height = 120, Margin = new Thickness(0, 0, 8, 0), Tag = photo.PhotoId };
            Image img = new Image() { Width = 120, Height = 120, Stretch = Stretch.UniformToFill, Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri("file:///" + photo.Location.Replace("\\", "/"))) };
            Button deleteBtn = CreateDeletePhotoButton(photo.PhotoId);
            slot.Children.Add(img);
            slot.Children.Add(deleteBtn);
            return slot;
        }

        private Button CreateDeletePhotoButton(int photoId)
        {
            Button deleteBtn = new Button()
            {
                Content = "✕",
                Width = 28,
                Height = 28,
                Padding = new Thickness(0),
                CornerRadius = new CornerRadius(14),
                Background = new SolidColorBrush(Microsoft.UI.Colors.Red),
                Foreground = new SolidColorBrush(Microsoft.UI.Colors.White),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, -8, -8, 0),
                IsEnabled = ViewModel.CanRemovePhoto(),
                CanDrag = false,
                AllowDrop = false
            };
            deleteBtn.Click += (s, e) => { if (ViewModel.CanRemovePhoto()) { ViewModel.RemovePhoto(photoId); RenderPhotos(); } };
            return deleteBtn;
        }

        private void RefreshAllUI()
        {
            RenderPhotos();
            RenderInterests();
            RenderPreferredGenders();
            UpdateInterestsHeader();
            UpdateBioHeader();
            UpdateDistanceHeader();
            UpdateArchivedBanner();
            UpdateQuestionnaireButton();
            DrawMap();
        }

        private void RenderPhotos()
        {
            _isRenderingPhotos = true;

            PhotosListView.Items.Clear();

            foreach (Photo photo in ViewModel.Photos)
            {
                Grid slot = CreatePhotoSlot(photo);
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

            int emptySlots = ViewModel.Photos.Count < 6
                ? 6 - ViewModel.Photos.Count - 1
                : 0;

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

            _isRenderingPhotos = false;
        }

        private void PhotoItems_VectorChanged(Windows.Foundation.Collections.IObservableVector<object> sender, Windows.Foundation.Collections.IVectorChangedEventArgs e)
        {
            if (_isRenderingPhotos || ViewModel == null)
                return;

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
                btn.Style = isSelected
                    ? (Style)Resources["SelectedInterestButtonStyle"]
                    : (Style)Resources["UnselectedInterestButtonStyle"];

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
            if (ViewModel == null) return;

            InterestsPanel.Children.Clear();

            foreach (string interest in ViewModel.AllInterests)
            {
                bool isSelected = ViewModel.Interests
                    .Any(i => i.Trim().Equals(interest.Trim(), StringComparison.OrdinalIgnoreCase));

                Button btn = new Button();
                btn.Content = interest;
                btn.IsEnabled = true;
                btn.Style = isSelected
                    ? (Style)Resources["SelectedInterestButtonStyle"]
                    : (Style)Resources["UnselectedInterestButtonStyle"];

                string interestCopy = interest;
                btn.Click += (s, e) =>
                {
                    string? existingInterest = ViewModel.Interests
                        .FirstOrDefault(i => i.Trim().Equals(interestCopy.Trim(), StringComparison.OrdinalIgnoreCase));

                    if (existingInterest != null)
                    {
                        if (ViewModel.CanRemoveInterest(existingInterest))
                            ViewModel.RemoveInterest(existingInterest);
                    }
                    else
                    {
                        if (ViewModel.CanAddInterest())
                            ViewModel.AddInterest(interestCopy);
                    }

                    RenderInterests();
                    UpdateInterestsHeader();
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
            BioHeader.Text = $"Bio ({ViewModel.Bio.Length}/250)";
        }

        private void UpdateDistanceHeader()
        {
            if (ViewModel != null)
            {
                DistanceHeader.Text = $"Maximum Distance ({ViewModel.MaxDistance} km)";
            }
        }

        private void DistanceSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            UpdateDistanceHeader();
            DrawMap();
        }

        private void LoadLocations()
        {
            LocationUtil locationUtil = new LocationUtil();
            List<string> locations = locationUtil.GetAllLocations();
            foreach (string location in locations)
            {
                LocationComboBox.Items.Add(location);
            }
            SyncLocationToComboBox();
        }

        private void SyncLocationToComboBox()
        {
            if (ViewModel?.Location != null && !string.IsNullOrEmpty(ViewModel.Location))
            {
                int index = -1;
                for (int i = 0; i < LocationComboBox.Items.Count; i++)
                {
                    if (LocationComboBox.Items[i] is string item && item.Equals(ViewModel.Location, StringComparison.OrdinalIgnoreCase))
                    {
                        index = i;
                        break;
                    }
                }

                if (index >= 0)
                {
                    LocationComboBox.SelectedIndex = index;
                }
            }
        }

        private void HandleLocationChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null || LocationComboBox.SelectedItem == null)
                return;

            ViewModel.Location = LocationComboBox.SelectedItem as string ?? string.Empty;
            LoadCityCoordinates();
            DrawMap();
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
                ViewModel.SaveChangesCommand.Execute(null);
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
                ViewModel.DiscardChangesCommand.Execute(null);
                RefreshAllUI();
            }
        }

        private async void ArchiveProfile_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Archive Profile";
            dialog.Content = "Are you sure you want to archive your profile? It will not be visible to other users.";
            dialog.PrimaryButtonText = "Archive";
            dialog.CloseButtonText = "Cancel";
            dialog.XamlRoot = this.XamlRoot;

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.ArchiveProfileCommand.Execute(null);
                UpdateArchivedBanner();
            }
        }

        private async void UnarchiveProfile_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Unarchive Profile";
            dialog.Content = "Are you sure you want to unarchive your profile? It will be visible to other users again.";
            dialog.PrimaryButtonText = "Unarchive";
            dialog.CloseButtonText = "Cancel";
            dialog.XamlRoot = this.XamlRoot;

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.UnarchiveProfileCommand.Execute(null);
                UpdateArchivedBanner();
            }
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
                ViewModel.DeleteProfileCommand.Execute(null);

                Window window = (Application.Current as App)!._window!;
                if (window.Content is Grid grid && grid.Children.Count > 0 && grid.Children[0] is Frame rootFrame)
                {
                    var profileRepo = new Repositories.ProfileRepository(App.ConnectionString);
                    var adminRepo = new Repositories.DatingAdminRepository(App.ConnectionString);
                    var mockUserUtil = new Utils.MockUserUtil();

                    var profileService = new Services.ProfileService(profileRepo, mockUserUtil);
                    var adminService = new Services.DatingAdminService(adminRepo);

                    var splashViewModel = new ViewModels.SplashViewModel(ViewModel._userId, mockUserUtil, profileService, adminService);
                    var createProfileViewModel = new ViewModels.CreateProfileViewModel(ViewModel._userId, profileService, mockUserUtil);

                    rootFrame.Navigate(typeof(SplashView));
                    if (rootFrame.Content is SplashView splashView)
                    {
                        splashView.SetViewModel(splashViewModel, createProfileViewModel);
                    }
                }
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
            Frame.Navigate(typeof(QuestionnaireView), ViewModel);
        }

        private void UpdateQuestionnaireButton()
        {
            QuestionnaireButton.Visibility = ViewModel.HasLoverType
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void LoadCityCoordinates()
        {
            _userCityCoords = null;
            if (ViewModel == null) return;

            string cityName = ViewModel.Location?.Trim() ?? "";
            if (string.IsNullOrEmpty(cityName)) return;

            string csvPath = System.IO.Path.Combine(AppContext.BaseDirectory, "locations.csv");
            if (!File.Exists(csvPath)) return;

            foreach (string line in File.ReadAllLines(csvPath))
            {
                string[] parts = line.Split(',');
                if (parts.Length < 3) continue;

                if (parts[0].Trim().Equals(cityName, StringComparison.OrdinalIgnoreCase))
                {
                    if (double.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double lat) &&
                        double.TryParse(parts[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double lon))
                    {
                        _userCityCoords = (lat, lon);
                        return;
                    }
                }
            }
        }

  
        private (double x, double y) ProjectToCanvas(double lat, double lon, double canvasW, double canvasH)
        {
            double pixelLeft = ImgMarginLeft * canvasW;
            double pixelRight = ImgMarginRight * canvasW;
            double pixelTop = ImgMarginTop * canvasH;
            double pixelBottom = ImgMarginBottom * canvasH;

            double x = pixelLeft + (lon - MapLonWest) / (MapLonEast - MapLonWest) * (pixelRight - pixelLeft);
            double y = pixelTop + (MapLatNorth - lat) / (MapLatNorth - MapLatSouth) * (pixelBottom - pixelTop);
            return (x, y);
        }

        private void DrawMap()
        {
            if (ViewModel == null || MapCanvas == null) return;

            MapCanvas.Children.Clear();

            if (_userCityCoords == null) return;

            double w = MapCanvas.ActualWidth;
            double h = MapCanvas.ActualHeight;
            if (w <= 0 || h <= 0) return;

            var (cx, cy) = ProjectToCanvas(_userCityCoords.Value.lat, _userCityCoords.Value.lon, w, h);

            double pixelsPerDegreeLat = (ImgMarginBottom - ImgMarginTop) * h / (MapLatNorth - MapLatSouth);
            double degreesLat = ViewModel.MaxDistance / 111.0;
            double pixelRadius = degreesLat * pixelsPerDegreeLat;

            var circle = new Ellipse
            {
                Width = pixelRadius * 2,
                Height = pixelRadius * 2,
                Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(55, 201, 127, 157)),
                Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb(180, 201, 127, 157)),
                StrokeThickness = 1.5
            };
            Canvas.SetLeft(circle, cx - pixelRadius);
            Canvas.SetTop(circle, cy - pixelRadius);
            MapCanvas.Children.Add(circle);

            double outerSize = 14;
            var pinOuter = new Ellipse
            {
                Width = outerSize,
                Height = outerSize,
                Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)),
                Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 180, 50, 90)),
                StrokeThickness = 2
            };
            Canvas.SetLeft(pinOuter, cx - outerSize / 2);
            Canvas.SetTop(pinOuter, cy - outerSize / 2);
            MapCanvas.Children.Add(pinOuter);

            double innerSize = 7;
            var pinInner = new Ellipse
            {
                Width = innerSize,
                Height = innerSize,
                Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 180, 50, 90))
            };
            Canvas.SetLeft(pinInner, cx - innerSize / 2);
            Canvas.SetTop(pinInner, cy - innerSize / 2);
            MapCanvas.Children.Add(pinInner);
        }

        private void MapCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawMap();
        }


    }
}