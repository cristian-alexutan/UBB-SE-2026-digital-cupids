using matchmaking.Domain;
using matchmaking.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;

namespace matchmaking.Views
{
    internal sealed partial class NotificationsView : Page
    {
        internal NotificationsViewModel? ViewModel { get; private set; }

        public NotificationsView()
        {
            InitializeComponent();
        }

        internal void SetViewModel(NotificationsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            ViewModel?.LoadNotifications();
            NotificationsList.Items.Clear();

            if (ViewModel == null) return;

            foreach (Notification notification in ViewModel.Notifications)
            {
                Border banner = CreateNotificationBanner(notification);
                NotificationsList.Items.Add(banner);
            }
        }

        private Border CreateNotificationBanner(Notification notification)
        {
            string glyph = notification.Type == NotificationType.MATCH ? "\uEB51" : "\uE735";
            SolidColorBrush iconColor = notification.Type == NotificationType.MATCH
                ? new SolidColorBrush(ColorHelper.FromArgb(255, 255, 105, 180))
                : new SolidColorBrush(ColorHelper.FromArgb(255, 0, 164, 255));

            SolidColorBrush background = notification.IsRead
                ? new SolidColorBrush(Colors.Transparent)
                : new SolidColorBrush(ColorHelper.FromArgb(30, 255, 90, 95));

            TimeSpan diff = DateTime.UtcNow - notification.CreatedAt;
            string timeText;
            if (diff.TotalMinutes < 1)
                timeText = "Just now";
            else if (diff.TotalHours < 1)
                timeText = $"{(int)diff.TotalMinutes} minutes ago";
            else if (diff.TotalHours < 24)
                timeText = $"{(int)diff.TotalHours} hours ago";
            else
                timeText = notification.CreatedAt.ToString("dd/MM/yyyy HH:mm");

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            FontIcon iconBlock = new FontIcon
            {
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                Glyph = glyph,
                FontSize = 28,
                Foreground = iconColor,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 16, 0)
            };
            Grid.SetColumn(iconBlock, 0);

            StackPanel textPanel = new StackPanel { Spacing = 4, VerticalAlignment = VerticalAlignment.Center };
            textPanel.Children.Add(new TextBlock
            {
                Text = notification.Title,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                FontSize = 16
            });
            textPanel.Children.Add(new TextBlock
            {
                Text = notification.Description,
                FontSize = 13,
                Foreground = new SolidColorBrush(Colors.Gray)
            });
            Grid.SetColumn(textPanel, 1);

            TextBlock timeBlock = new TextBlock
            {
                Text = timeText,
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.Gray),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(16, 0, 0, 0)
            };
            Grid.SetColumn(timeBlock, 2);

            grid.Children.Add(iconBlock);
            grid.Children.Add(textPanel);
            grid.Children.Add(timeBlock);

            Border border = new Border
            {
                Padding = new Thickness(16),
                Margin = new Thickness(0, 6, 0, 6),
                CornerRadius = new CornerRadius(12),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(ColorHelper.FromArgb(255, 255, 90, 95)),
                Background = background,
                Tag = notification.NotificationId
            };
            border.Child = grid;
            border.Tapped += HandleNotificationTapped;

            return border;
        }

        private async void HandleNotificationTapped(object sender, TappedRoutedEventArgs e)
        {
            Border border = (Border)sender;
            int notificationId = (int)border.Tag;

            ContentDialog dialog = new ContentDialog
            {
                Title = "Notification options",
                PrimaryButtonText = "Mark as read",
                SecondaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel?.MarkAsRead(notificationId);
                LoadNotifications();
            }
            else if (result == ContentDialogResult.Secondary)
            {
                ViewModel?.Delete(notificationId);
                LoadNotifications();
            }
        }

        private void HandleMarkAllAsReadClick(object sender, RoutedEventArgs e)
        {
            ViewModel?.MarkAllAsRead();
            LoadNotifications();
        }

        private async void HandleDeleteAllClick(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Delete all notifications",
                Content = "Are you sure you want to delete all notifications?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                XamlRoot = this.XamlRoot
            };

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ViewModel?.DeleteAll();
                LoadNotifications();
            }
        }
    }
}