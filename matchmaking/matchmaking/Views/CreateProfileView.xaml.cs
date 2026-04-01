using matchmaking.Domain;
using matchmaking.Services;
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
                ViewModel.LoadUserData(ViewModel.UserId);
                UsernameText.Text = GetUsername();
                AgeText.Text = GetAge().ToString();
                LoadLocations();
                LoadInterests();
                UpdatePhotoSlots();
                UpdateNextButton();
            }
        }

        private string GetUsername()
        {
            MockUserUtil userUtil = new MockUserUtil();
            return userUtil.GetUserData(ViewModel!.UserId).Username;
        }

        private int GetAge()
        {
            MockUserUtil userUtil = new MockUserUtil();
            DateTime birthDate = userUtil.GetUserData(ViewModel!.UserId).Birthdate;
            int age = DateTime.Now.Year - birthDate.Year;

            if (DateTime.Now < birthDate.AddYears(age))
            {
                age--;
            }
            return age;
        }

        private void HandleNextStepClick(object sender, RoutedEventArgs e)
        {
            SyncUItoViewModel();
            ViewModel!.NextStep();
            UpdateStepUI();
        }

        private void HandlePreviousStepClick(object sender, RoutedEventArgs e)
        {
            SyncUItoViewModel();
            ViewModel!.PreviousStep();
            UpdateStepUI();
        }

        private void SyncUItoViewModel()
        {
            if (ViewModel?.ProfileData == null)
            {
                return;
            }

            int step = ViewModel.CurrentStep;

            if (step == 1)
            {
                ViewModel.ProfileData.Location = LocationComboBox.SelectedItem as string ?? string.Empty;
                ViewModel.ProfileData.Nationality = NationalityTextBox.Text;
                ViewModel.ProfileData.Gender = GenderComboBox.SelectedIndex switch
                {
                    0 => Gender.MALE,
                    1 => Gender.FEMALE,
                    2 => Gender.NON_BINARY,
                    _ => Gender.OTHER
                };

                ViewModel.ProfileData.PreferredGenders = new List<Gender>();

                if (PrefMaleCheckBox.IsChecked == true)
                {
                    ViewModel.ProfileData.PreferredGenders.Add(Gender.MALE);
                }

                if (PrefFemaleCheckBox.IsChecked == true)
                {
                    ViewModel.ProfileData.PreferredGenders.Add(Gender.FEMALE);
                }

                if (PrefNonBinaryCheckBox.IsChecked == true)
                {
                    ViewModel.ProfileData.PreferredGenders.Add(Gender.NON_BINARY);
                }

                if (PrefOtherCheckBox.IsChecked == true)
                {
                    ViewModel.ProfileData.PreferredGenders.Add(Gender.OTHER);
                }

                ViewModel.ProfileData.MaxDistance = (int)MaxDistanceSlider.Value;
                ViewModel.ProfileData.MinPreferredAge = (int)AgeRangeSelector.RangeStart;
                ViewModel.ProfileData.MaxPreferredAge = (int)AgeRangeSelector.RangeEnd;
                ViewModel.ProfileData.DisplayStarSign = DisplayStarSignToggle.IsOn;
            }
            else if (step == 3)
            {
                ViewModel.ProfileData.Bio = BioTextBox.Text;
            }
        }

        private void SyncViewModeltoUI()
        {
            if (ViewModel?.ProfileData == null)
            {
                return;
            }

            int step = ViewModel.CurrentStep;

            if (step == 1)
            {
                LocationComboBox.SelectedItem = ViewModel.ProfileData.Location;
                NationalityTextBox.Text = ViewModel.ProfileData.Nationality;
                GenderComboBox.SelectedIndex = ViewModel.ProfileData.Gender switch
                {
                    Gender.MALE => 0,
                    Gender.FEMALE => 1,
                    Gender.NON_BINARY => 2,
                    _ => 3
                };
                PrefMaleCheckBox.IsChecked = ViewModel.ProfileData.PreferredGenders.Contains(Gender.MALE);
                PrefFemaleCheckBox.IsChecked = ViewModel.ProfileData.PreferredGenders.Contains(Gender.FEMALE);
                PrefNonBinaryCheckBox.IsChecked = ViewModel.ProfileData.PreferredGenders.Contains(Gender.NON_BINARY);
                PrefOtherCheckBox.IsChecked = ViewModel.ProfileData.PreferredGenders.Contains(Gender.OTHER);
                MaxDistanceSlider.Value = ViewModel.ProfileData.MaxDistance;
                AgeRangeSelector.RangeStart = ViewModel.ProfileData.MinPreferredAge;
                AgeRangeSelector.RangeEnd = ViewModel.ProfileData.MaxPreferredAge;
                MaxDistanceValueText.Text = ViewModel.ProfileData.MaxDistance.ToString();
                MinAgeValueText.Text = ViewModel.ProfileData.MinPreferredAge.ToString();
                MaxAgeValueText.Text = ViewModel.ProfileData.MaxPreferredAge.ToString();
                DisplayStarSignToggle.IsOn = ViewModel.ProfileData.DisplayStarSign;
            }
            else if (step == 3)
            {
                BioTextBox.Text = ViewModel.ProfileData.Bio;
                BioLengthText.Text = ViewModel.ProfileData.Bio.Length.ToString();
            }
        }

        private void UpdateStepUI()
        {
            int step = ViewModel!.CurrentStep;

            Step1Panel.Visibility = step == 1 ? Visibility.Visible : Visibility.Collapsed;
            Step2Panel.Visibility = step == 2 ? Visibility.Visible : Visibility.Collapsed;
            Step3Panel.Visibility = step == 3 ? Visibility.Visible : Visibility.Collapsed;
            Step4Panel.Visibility = step == 4 ? Visibility.Visible : Visibility.Collapsed;

            PreviousStepButton.IsEnabled = step != 1;
            NextStepButton.Visibility = step == 4 ? Visibility.Collapsed : Visibility.Visible;
            CreateProfileButton.Visibility = step == 4 ? Visibility.Visible : Visibility.Collapsed;

            Step1Dot.Fill = step == 1 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
                                      : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(128, 255, 255, 255));
            Step2Dot.Fill = step == 2 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
                                      : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(128, 255, 255, 255));
            Step3Dot.Fill = step == 3 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
                                      : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(128, 255, 255, 255));
            Step4Dot.Fill = step == 4 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
                                      : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(128, 255, 255, 255));

            SyncViewModeltoUI();

            if (step == 2)
                UpdatePhotoSlots();

            if (step == 4)
                UpdatePreview();

            UpdateNextButton();
        }

        private void UpdateNextButton()
        {
            if (ViewModel?.ProfileData == null)
            {
                return;
            }

            int step = ViewModel.CurrentStep;

            NextStepButton.IsEnabled = step switch
            {
                1 => IsStep1Valid(),
                2 => IsStep2Valid(),
                3 => IsStep3Valid(),
                _ => true
            };
        }

        private bool IsStep1Valid()
        {
            return !string.IsNullOrWhiteSpace(ViewModel!.ProfileData!.Location) &&
                   !string.IsNullOrWhiteSpace(ViewModel.ProfileData.Nationality) &&
                   GenderComboBox.SelectedIndex >= 0 &&
                   (PrefMaleCheckBox.IsChecked == true ||
                    PrefFemaleCheckBox.IsChecked == true ||
                    PrefNonBinaryCheckBox.IsChecked == true ||
                    PrefOtherCheckBox.IsChecked == true);
        }

        private bool IsStep2Valid()
        {
            return ViewModel!.ProfileData!.Photos.Count >= 2;
        }

        private bool IsStep3Valid()
        {
            return ViewModel!.ProfileData!.Bio.Length >= 20 &&
                   ViewModel.ProfileData.Bio.Length <= 250 &&
                   ViewModel.ProfileData.Interests.Count >= 3;
        }

        private void LoadLocations()
        {
            LocationUtil locationUtil = new LocationUtil();
            List<string> locations = locationUtil.GetAllLocations();
            foreach (string location in locations)
            {
                LocationComboBox.Items.Add(location);
            }   
        }

        private void HandleLocationChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel?.ProfileData == null)
            {
                return;
            }
            if (LocationComboBox.SelectedItem != null)
            {
                ViewModel.ProfileData.Location = LocationComboBox.SelectedItem as string;
            }
            else
            {
                ViewModel.ProfileData.Location = string.Empty;
            }
            UpdateNextButton();
        }

        private void HandleNationalityChanged(object sender, TextChangedEventArgs e)
        {
            if (ViewModel?.ProfileData == null)
            {
                return;
            }
            ViewModel.ProfileData.Nationality = NationalityTextBox.Text;
            UpdateNextButton();
        }

        private void HandleGenderChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel?.ProfileData == null)
            {
                return;
            }
            ViewModel.ProfileData.Gender = GenderComboBox.SelectedIndex switch
            {
                0 => Gender.MALE,
                1 => Gender.FEMALE,
                2 => Gender.NON_BINARY,
                _ => Gender.OTHER
            };
            UpdateNextButton();
        }

        private void HandlePreferredGenderChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.ProfileData == null)
            {
                return;
            }
            ViewModel.ProfileData.PreferredGenders = new List<Gender>();

            if (PrefMaleCheckBox.IsChecked == true)
            {
                ViewModel.ProfileData.PreferredGenders.Add(Gender.MALE);
            }
            if (PrefFemaleCheckBox.IsChecked == true)
            {
                ViewModel.ProfileData.PreferredGenders.Add(Gender.FEMALE);
            }
            if (PrefNonBinaryCheckBox.IsChecked == true)
            {
                ViewModel.ProfileData.PreferredGenders.Add(Gender.NON_BINARY);
            }
            if (PrefOtherCheckBox.IsChecked == true)
            {
                ViewModel.ProfileData.PreferredGenders.Add(Gender.OTHER);
            }
            UpdateNextButton();
        }

        private void HandleMaxDistanceChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (ViewModel?.ProfileData == null)
            {
                return;
            }
            ViewModel.ProfileData.MaxDistance = (int)MaxDistanceSlider.Value;
            MaxDistanceValueText.Text = ((int)MaxDistanceSlider.Value).ToString();
        }

        private void HandleAgeRangeChanged(object sender, object e)
        {
            if (ViewModel?.ProfileData == null)
            {
                return;
            }
            ViewModel.ProfileData.MinPreferredAge = (int)AgeRangeSelector.RangeStart;
            ViewModel.ProfileData.MaxPreferredAge = (int)AgeRangeSelector.RangeEnd;
            MinAgeValueText.Text = ((int)AgeRangeSelector.RangeStart).ToString();
            MaxAgeValueText.Text = ((int)AgeRangeSelector.RangeEnd).ToString();
        }


        private async void HandleAddPhotoClick(object sender, RoutedEventArgs e)
        {
            int slotIndex = int.Parse((sender as Button)!.Tag.ToString()!);

            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(
                (Application.Current as App)!._window);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                return;
            }

            BasicProperties properties = await file.GetBasicPropertiesAsync();
            if (properties.Size > 10 * 1024 * 1024)
            {
                ContentDialog sizeDialog = new ContentDialog
                {
                    Title = "File Too Large",
                    Content = "The selected file exceeds the 10MB limit.",
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await sizeDialog.ShowAsync();
                return;
            }

            Photo photo = new Photo(ViewModel!.UserId, file.Path, slotIndex);

            try
            {
                ViewModel.AddPhoto(photo);
                UpdatePhotoSlots();
                UpdateNextButton();
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }

        private async void HandleRemovePhotoClick(object sender, RoutedEventArgs e)
        {
            int photoId = (int)(sender as Button)!.Tag;
            try
            {
                ViewModel!.RemovePhoto(photoId);
                UpdatePhotoSlots();
                UpdateNextButton();
            }
            catch (Exception ex)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await dialog.ShowAsync();
            }
        }

        private void UpdatePhotoSlots()
        {
            Border[] slots = new Border[] { PhotoSlot0, PhotoSlot1, PhotoSlot2, PhotoSlot3, PhotoSlot4, PhotoSlot5 };
            List<Photo> photos = ViewModel!.ProfileData!.Photos;
            bool canRemove = photos.Count > 2;
            bool canAdd = photos.Count < 6;

            SolidColorBrush transparentBrush = new SolidColorBrush(Colors.Transparent);
            SolidColorBrush whiteBrush = new SolidColorBrush(Colors.White);
            SolidColorBrush removeBtnBackground = new SolidColorBrush(ColorHelper.FromArgb(180, 0, 0, 0));
            SolidColorBrush addBtnBorder = new SolidColorBrush(ColorHelper.FromArgb(255, 200, 200, 200));
            SolidColorBrush addBtnIcon = new SolidColorBrush(ColorHelper.FromArgb(255, 180, 180, 180));

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Child = null;
                slots[i].AllowDrop = true;
                slots[i].Background = transparentBrush;
                slots[i].DragOver -= HandleSlotDragOver;
                slots[i].Drop -= HandleSlotDrop;
                slots[i].DragOver += HandleSlotDragOver;
                slots[i].Drop += HandleSlotDrop;

                if (i < photos.Count)
                {
                    Grid grid = new Grid();

                    Grid dragGrid = new Grid();
                    dragGrid.Background = transparentBrush;
                    dragGrid.CanDrag = true;

                    int slotIndex = i;
                    dragGrid.DragStarting += (s, args) =>
                    {
                        args.Data.SetText(slotIndex.ToString());
                        args.Data.RequestedOperation = DataPackageOperation.Move;
                    };

                    Image img = new Image
                    {
                        Source = new BitmapImage(new Uri(photos[i].Location!)),
                        Stretch = Stretch.UniformToFill
                    };
                    dragGrid.Children.Add(img);

                    Button removeBtn = new Button
                    {
                        Content = new FontIcon { Glyph = "\uE894", FontSize = 16 },
                        Tag = photos[i].PhotoId,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        Background = removeBtnBackground,
                        Foreground = whiteBrush,
                        Width = 32,
                        Height = 32,
                        CornerRadius = new CornerRadius(16),
                        Margin = new Thickness(0, 4, 4, 0),
                        IsEnabled = canRemove
                    };
                    removeBtn.Click += HandleRemovePhotoClick;

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
                        Background = transparentBrush,
                        BorderBrush = addBtnBorder,
                        BorderThickness = new Thickness(2),
                        Content = new FontIcon
                        {
                            Glyph = "\uE710",
                            FontSize = 32,
                            Foreground = addBtnIcon
                        },
                        IsEnabled = canAdd
                    };
                    addBtn.Click += HandleAddPhotoClick;
                    slots[i].Child = addBtn;
                }
            }
        }

        private void HandleSlotDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
        }

        private void HandleSlotDrop(object sender, DragEventArgs e)
        {
            Border[] slots = new Border[] { PhotoSlot0, PhotoSlot1, PhotoSlot2, PhotoSlot3, PhotoSlot4, PhotoSlot5 };
            int targetSlot = Array.IndexOf(slots, sender as Border);

            string? sourceText = e.DataView.GetTextAsync().GetAwaiter().GetResult();
            if (string.IsNullOrEmpty(sourceText) || !int.TryParse(sourceText, out int sourceSlot))
            {
                return;
            }

            int photoCount = ViewModel!.ProfileData!.Photos.Count;
            bool slotsAreDifferent = sourceSlot != targetSlot;
            bool slotsAreValid = sourceSlot >= 0 && targetSlot >= 0;
            bool slotsAreWithinBounds = sourceSlot < photoCount && targetSlot < photoCount;

            if (slotsAreDifferent && slotsAreValid && slotsAreWithinBounds)
            {
                ViewModel.SwapPhotos(sourceSlot, targetSlot);
                UpdatePhotoSlots();
            }
        }

        private void HandleBioTextChanged(object sender, TextChangedEventArgs e)
        {
            if (ViewModel?.ProfileData == null)
            {
                return;
            }
            ViewModel.ProfileData.Bio = BioTextBox.Text;
            BioLengthText.Text = BioTextBox.Text.Length.ToString();
            UpdateNextButton();
        }

        private void LoadInterests()
        {
            InterestUtil interestUtil = new InterestUtil();
            List<string> allInterests = interestUtil.GetAll();
            List<Button> buttons = new List<Button>();

            SolidColorBrush accentBrush = new SolidColorBrush(ColorHelper.FromArgb(255, 235, 59, 89));
            SolidColorBrush whiteBrush = new SolidColorBrush(ColorHelper.FromArgb(255, 255, 255, 255));

            foreach (string interest in allInterests)
            {
                Button btn = new Button
                {
                    Content = interest,
                    Tag = interest,
                    CornerRadius = new CornerRadius(20),
                    Padding = new Thickness(16, 8, 16, 8),
                    BorderThickness = new Thickness(2),
                    BorderBrush = accentBrush,
                    Background = whiteBrush,
                    Foreground = accentBrush,
                    FontWeight = FontWeights.SemiBold
                };
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

            SolidColorBrush accentBrush = new SolidColorBrush(ColorHelper.FromArgb(255, 235, 59, 89));
            SolidColorBrush whiteBrush = new SolidColorBrush(Colors.White);

            try
            {
                if (isSelected)
                {
                    ViewModel!.RemoveInterest(interest);
                    btn.Background = whiteBrush;
                    btn.Foreground = accentBrush;
                    btn.BorderBrush = accentBrush;
                }
                else
                {
                    ViewModel!.AddInterest(interest);
                    btn.Background = accentBrush;
                    btn.Foreground = whiteBrush;
                    btn.BorderBrush = accentBrush;
                }

                InterestCountText.Text = ViewModel!.ProfileData!.Interests.Count.ToString();
                UpdateNextButton();
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await errorDialog.ShowAsync();
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

            if (preview.DisplayStarSign)
            {
                PreviewStarSignPanel.Visibility = Visibility.Visible;
                PreviewStarSign.Text = preview.GetStarSign().ToString();
            }
            else
            {
                PreviewStarSignPanel.Visibility = Visibility.Collapsed;
            }

            if (preview.Photos.Count > 0)
            {
                PreviewPhotoImage.Source = new BitmapImage(new Uri(preview.Photos[0].Location!));
            }
        }

        private void HandleNextPhotoClick(object sender, RoutedEventArgs e)
        {
            ViewModel!.NextPhoto();
            List<Photo> photos = ViewModel.ProfileData!.Photos;
            if (photos.Count > 0)
            {
                PreviewPhotoImage.Source = new BitmapImage(new Uri(photos[ViewModel.CurrentPhotoIndex].Location!));
            }
        }

        private void HandlePreviousPhotoClick(object sender, RoutedEventArgs e)
        {
            ViewModel!.PreviousPhoto();
            List<Photo> photos = ViewModel.ProfileData!.Photos;
            if (photos.Count > 0)
            {
                PreviewPhotoImage.Source = new BitmapImage(new Uri(photos[ViewModel.CurrentPhotoIndex].Location!));
            }
        }

        private void HandleTermsConditionsChecked(object sender, RoutedEventArgs e)
        {
            if (TermsCheckBox.IsChecked == true)
            {
                ViewModel!.TermsAccepted = true;
            }
            else
            {
                ViewModel!.TermsAccepted = false;
            }
            CreateProfileButton.IsEnabled = ViewModel.TermsAccepted;
        }

        private async void HandleCreateDatingProfileClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel!.CreateDatingProfile();
                Frame.Navigate(typeof(DiscoverView));
            }
            catch (Exception ex)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await dialog.ShowAsync();
            }
        }
    }
}
