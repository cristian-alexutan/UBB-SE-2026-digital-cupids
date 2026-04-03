using matchmaking.Domain;
using matchmaking.Utils;
using matchmaking.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;

namespace matchmaking.Views
{
    internal sealed partial class CreateProfileView : Page
    {
        internal CreateProfileViewModel? ViewModel { get; private set; }

        public CreateProfileView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is CreateProfileViewModel viewModel)
            {
                ViewModel = viewModel;
                ViewModel.PropertyChanged += OnViewModelPropertyChanged;
                ViewModel.ProfileCreated += OnProfileCreated;
                ViewModel.ErrorOccurred += OnErrorOccurred;
                ViewModel.LoadUserData(ViewModel.UserId);
                AgeText.Text = GetAge().ToString();
                LoadLocations();
                LoadInterests();
                UpdatePhotoSlots();
                UpdateNextButton();
            }
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.CurrentStep))
            {
                UpdateStepUI();
            }
            else if (e.PropertyName == nameof(ViewModel.CurrentPhotoIndex))
            {
                List<Photo> photos = ViewModel!.ProfileData!.Photos;
                if (photos.Count > 0)
                    PreviewPhotoImage.Source = new BitmapImage(new Uri(photos[ViewModel.CurrentPhotoIndex].Location!));
            }
            else if (e.PropertyName == nameof(ViewModel.MaxDistance))
            {
                MaxDistanceValueText.Text = ((int)ViewModel!.MaxDistance).ToString();
            }
            else if (e.PropertyName == nameof(ViewModel.MinPreferredAge))
            {
                MinAgeValueText.Text = ((int)ViewModel!.MinPreferredAge).ToString();
            }
            else if (e.PropertyName == nameof(ViewModel.MaxPreferredAge))
            {
                MaxAgeValueText.Text = ((int)ViewModel!.MaxPreferredAge).ToString();
            }
            else if (e.PropertyName == nameof(ViewModel.BioLength))
            {
                BioLengthText.Text = ViewModel!.BioLength.ToString();
            }
            else if (e.PropertyName == nameof(ViewModel.ProfileData))
            {
                if (ViewModel!.CurrentStep == 2)
                    UpdatePhotoSlots();
            }

            UpdateNextButton();
        }

        private void OnProfileCreated()
        {
            var mainViewModel = new MainViewModel(ViewModel!.UserId, App.ConnectionString, true);
            Frame.Navigate(typeof(MainView), mainViewModel);
        }

        private async void OnErrorOccurred(string message)
        {
            await new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = XamlRoot
            }.ShowAsync();
        }

        private int GetAge()
        {
            MockUserUtil userUtil = new MockUserUtil();
            DateTime birthDate = userUtil.GetUserData(ViewModel!.UserId).Birthdate;
            int age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now < birthDate.AddYears(age)) age--;
            return age;
        }

        private void SyncViewModeltoUI()
        {
            if (ViewModel?.ProfileData == null) return;

            int step = ViewModel.CurrentStep;

            if (step == 1)
            {
                PrefMaleCheckBox.IsChecked = ViewModel.ProfileData.PreferredGenders.Contains(Gender.MALE);
                PrefFemaleCheckBox.IsChecked = ViewModel.ProfileData.PreferredGenders.Contains(Gender.FEMALE);
                PrefNonBinaryCheckBox.IsChecked = ViewModel.ProfileData.PreferredGenders.Contains(Gender.NON_BINARY);
                PrefOtherCheckBox.IsChecked = ViewModel.ProfileData.PreferredGenders.Contains(Gender.OTHER);
                MaxDistanceValueText.Text = ((int)ViewModel.MaxDistance).ToString();
                MinAgeValueText.Text = ((int)ViewModel.MinPreferredAge).ToString();
                MaxAgeValueText.Text = ((int)ViewModel.MaxPreferredAge).ToString();
            }
            else if (step == 3)
            {
                BioLengthText.Text = ViewModel.BioLength.ToString();
            }
        }

        private void UpdateStepUI()
        {
            int step = ViewModel!.CurrentStep;

            Step1Panel.Visibility = step == 1 ? Visibility.Visible : Visibility.Collapsed;
            Step2Panel.Visibility = step == 2 ? Visibility.Visible : Visibility.Collapsed;
            Step3Panel.Visibility = step == 3 ? Visibility.Visible : Visibility.Collapsed;
            Step4Panel.Visibility = step == 4 ? Visibility.Visible : Visibility.Collapsed;

            NextStepButton.Visibility = step == 4 ? Visibility.Collapsed : Visibility.Visible;
            CreateProfileButton.Visibility = step == 4 ? Visibility.Visible : Visibility.Collapsed;

            SolidColorBrush fullWhite = new SolidColorBrush(Colors.White);
            SolidColorBrush halfWhite = new SolidColorBrush(ColorHelper.FromArgb(128, 255, 255, 255));
            Step1Dot.Fill = step == 1 ? fullWhite : halfWhite;
            Step2Dot.Fill = step == 2 ? fullWhite : halfWhite;
            Step3Dot.Fill = step == 3 ? fullWhite : halfWhite;
            Step4Dot.Fill = step == 4 ? fullWhite : halfWhite;

            SyncViewModeltoUI();

            if (step == 2) UpdatePhotoSlots();
            if (step == 4) UpdatePreview();

            UpdateNextButton();
        }

        private void UpdateNextButton()
        {
            if (ViewModel?.ProfileData == null) return;

            NextStepButton.IsEnabled = ViewModel.CurrentStep switch
            {
                1 => IsStep1Valid(),
                2 => IsStep2Valid(),
                3 => IsStep3Valid(),
                _ => true
            };
        }

        private bool IsStep1Valid() =>
            !string.IsNullOrWhiteSpace(ViewModel!.Name) &&
            !string.IsNullOrWhiteSpace(ViewModel.Location) &&
            !string.IsNullOrWhiteSpace(ViewModel.Nationality) &&
            ViewModel.GenderIndex >= 0 &&
            (PrefMaleCheckBox.IsChecked == true ||
             PrefFemaleCheckBox.IsChecked == true ||
             PrefNonBinaryCheckBox.IsChecked == true ||
             PrefOtherCheckBox.IsChecked == true);

        private bool IsStep2Valid() => ViewModel!.ProfileData!.Photos.Count >= 2;

        private bool IsStep3Valid() =>
            ViewModel!.ProfileData!.Bio.Length >= 20 &&
            ViewModel.ProfileData.Bio.Length <= 250 &&
            ViewModel.ProfileData.Interests.Count >= 3;

        private void LoadLocations()
        {
            LocationUtil locationUtil = new LocationUtil();
            foreach (string location in locationUtil.GetAllLocations())
                LocationComboBox.Items.Add(location);
        }

        private void HandlePreferredGenderChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.ProfileData == null) return;
            ViewModel.ProfileData.PreferredGenders = new List<Gender>();
            if (PrefMaleCheckBox.IsChecked == true) ViewModel.ProfileData.PreferredGenders.Add(Gender.MALE);
            if (PrefFemaleCheckBox.IsChecked == true) ViewModel.ProfileData.PreferredGenders.Add(Gender.FEMALE);
            if (PrefNonBinaryCheckBox.IsChecked == true) ViewModel.ProfileData.PreferredGenders.Add(Gender.NON_BINARY);
            if (PrefOtherCheckBox.IsChecked == true) ViewModel.ProfileData.PreferredGenders.Add(Gender.OTHER);
            UpdateNextButton();
        }

        private async void HandleAddPhotoClick(object sender, RoutedEventArgs e)
        {
            int slotIndex = int.Parse((sender as Button)!.Tag.ToString()!);

            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle((Application.Current as App)!._window);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null) return;

            BasicProperties properties = await file.GetBasicPropertiesAsync();
            if (properties.Size > 10 * 1024 * 1024)
            {
                await new ContentDialog
                {
                    Title = "File Too Large",
                    Content = "The selected file exceeds the 10MB limit.",
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                }.ShowAsync();
                return;
            }

            try
            {
                ViewModel!.AddPhoto(new Photo(ViewModel.UserId, file.Path, slotIndex));
                UpdatePhotoSlots();
                UpdateNextButton();
            }
            catch (Exception ex)
            {
                await new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                }.ShowAsync();
            }
        }

        private void UpdatePhotoSlots()
        {
            Border[] slots = { PhotoSlot0, PhotoSlot1, PhotoSlot2, PhotoSlot3, PhotoSlot4, PhotoSlot5 };
            List<Photo> photos = ViewModel!.ProfileData!.Photos;
            bool canRemove = true;
            bool canAdd = photos.Count < 6;

            SolidColorBrush transparent = new SolidColorBrush(Colors.Transparent);
            SolidColorBrush white = new SolidColorBrush(Colors.White);
            SolidColorBrush removeBg = new SolidColorBrush(ColorHelper.FromArgb(180, 0, 0, 0));
            SolidColorBrush addBtnBorder = new SolidColorBrush(ColorHelper.FromArgb(255, 200, 200, 200));
            SolidColorBrush addBtnIcon = new SolidColorBrush(ColorHelper.FromArgb(255, 180, 180, 180));

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Child = null;
                slots[i].AllowDrop = true;
                slots[i].Background = transparent;
                slots[i].DragOver -= HandleSlotDragOver;
                slots[i].Drop -= HandleSlotDrop;
                slots[i].DragOver += HandleSlotDragOver;
                slots[i].Drop += HandleSlotDrop;

                if (i < photos.Count)
                {
                    Grid grid = new Grid();
                    Grid dragGrid = new Grid { Background = transparent, CanDrag = true };

                    int slotIndex = i;
                    dragGrid.DragStarting += (s, args) =>
                    {
                        args.Data.SetText(slotIndex.ToString());
                        args.Data.RequestedOperation = DataPackageOperation.Move;
                    };
                    dragGrid.Children.Add(new Image
                    {
                        Source = new BitmapImage(new Uri(photos[i].Location!)),
                        Stretch = Stretch.UniformToFill
                    });

                    Button removeBtn = new Button
                    {
                        Content = new FontIcon { Glyph = "\uE894", FontSize = 16 },
                        Command = ViewModel.RemovePhotoCommand,
                        CommandParameter = photos[i].PhotoId,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        Background = removeBg,
                        Foreground = white,
                        Width = 32,
                        Height = 32,
                        CornerRadius = new CornerRadius(16),
                        Margin = new Thickness(0, 4, 4, 0),
                    };

                    grid.Children.Add(dragGrid);
                    grid.Children.Add(removeBtn);
                    slots[i].Child = grid;
                }
                else
                {
                    Button addBtn = new Button
                    {
                        Tag = i.ToString(),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Background = transparent,
                        BorderBrush = addBtnBorder,
                        BorderThickness = new Thickness(2),
                        Content = new FontIcon { Glyph = "\uE710", FontSize = 32, Foreground = addBtnIcon },
                        IsEnabled = canAdd
                    };
                    addBtn.Click += HandleAddPhotoClick;
                    slots[i].Child = addBtn;
                }
            }
        }

        private void HandleSlotDragOver(object sender, DragEventArgs e)
            => e.AcceptedOperation = DataPackageOperation.Move;

        private void HandleSlotDrop(object sender, DragEventArgs e)
        {
            Border[] slots = { PhotoSlot0, PhotoSlot1, PhotoSlot2, PhotoSlot3, PhotoSlot4, PhotoSlot5 };
            int targetSlot = Array.IndexOf(slots, sender as Border);

            string? sourceText = e.DataView.GetTextAsync().GetAwaiter().GetResult();
            if (string.IsNullOrEmpty(sourceText) || !int.TryParse(sourceText, out int sourceSlot)) return;

            int photoCount = ViewModel!.ProfileData!.Photos.Count;
            if (sourceSlot != targetSlot && sourceSlot >= 0 && targetSlot >= 0
                && sourceSlot < photoCount && targetSlot < photoCount)
            {
                ViewModel.SwapPhotos(sourceSlot, targetSlot);
                UpdatePhotoSlots();
            }
        }

        private void LoadInterests()
        {
            InterestUtil interestUtil = new InterestUtil();
            SolidColorBrush accent = new SolidColorBrush(ColorHelper.FromArgb(255, 235, 59, 89));
            SolidColorBrush white = new SolidColorBrush(Colors.White);
            SolidColorBrush pink = new SolidColorBrush(ColorHelper.FromArgb(255, 255, 182, 193));

            List<Button> buttons = new List<Button>();
    
            foreach (string interest in interestUtil.GetAll())
            {
                Button btn = new Button
                {
                    Content = interest,
                    Tag = interest,
                    CornerRadius = new CornerRadius(20),
                    Padding = new Thickness(16, 8, 16, 8),
                    BorderThickness = new Thickness(2),
                    BorderBrush = accent,
                    Background = white,
                    Foreground = accent,
                    FontWeight = FontWeights.SemiBold
                };

                btn.Resources["ButtonBackgroundPointerOver"] = pink;
                btn.Resources["ButtonForegroundPointerOver"] = accent;
                btn.Resources["ButtonBorderBrushPointerOver"] = accent;

                btn.Click += HandleInterestClick;
                buttons.Add(btn);
            }
            InterestsRepeater.ItemsSource = buttons;
        }

        private async void HandleInterestClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string interest = (string)btn.Tag;
            bool isSelected = btn.Background is SolidColorBrush brush
                              && brush.Color == ColorHelper.FromArgb(255, 235, 59, 89);

            SolidColorBrush accent = new SolidColorBrush(ColorHelper.FromArgb(255, 235, 59, 89));
            SolidColorBrush white = new SolidColorBrush(Colors.White);
            SolidColorBrush pink = new SolidColorBrush(ColorHelper.FromArgb(255, 255, 182, 193));


            try
            {
                if (isSelected)
                {
                    ViewModel!.RemoveInterest(interest);
                    btn.Background = white;
                    btn.Foreground = accent;
                    btn.Resources["ButtonBackgroundPointerOver"] = pink;
                    btn.Resources["ButtonForegroundPointerOver"] = accent;
                }
                else
                {
                    ViewModel!.AddInterest(interest);
                    btn.Background = accent;
                    btn.Foreground = white;
                    btn.Resources["ButtonBackgroundPointerOver"] = pink;
                    btn.Resources["ButtonForegroundPointerOver"] = white;
                }
                btn.BorderBrush = accent;
                btn.Resources["ButtonBorderBrushPointerOver"] = accent;
                InterestCountText.Text = ViewModel!.ProfileData!.Interests.Count.ToString();
                UpdateNextButton();
            }
            catch (Exception ex)
            {
                await new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                }.ShowAsync();
            }
        }

        private void UpdatePreview()
        {
            DatingProfile preview = ViewModel!.GetPreviewProfile();
            PreviewNameAge.Text = $"{preview.Name}, {preview.Age}";
            PreviewAge.Text = preview.Age.ToString();
            PreviewGender.Text = preview.Gender.ToString();
            PreviewLocation.Text = preview.Location;
            PreviewNationality.Text = preview.Nationality;
            PreviewBio.Text = preview.Bio;
            PreviewInterestsControl.ItemsSource = preview.Interests;

            PreviewStarSignPanel.Visibility = preview.DisplayStarSign ? Visibility.Visible : Visibility.Collapsed;
            if (preview.DisplayStarSign)
                PreviewStarSign.Text = preview.GetStarSign().ToString();

            if (preview.Photos.Count > 0)
                PreviewPhotoImage.Source = new BitmapImage(new Uri(preview.Photos[0].Location!));
        }

    }
}