using matchmaking.Domain;
using matchmaking.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;

namespace matchmaking.Views
{
    public sealed partial class AdminView : Page
    {
        internal AdminViewModel ViewModel { get; private set; }

        public AdminView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = (AdminViewModel)e.Parameter;
        }

        private void Ticket_Expanding(Expander sender, ExpanderExpandingEventArgs args)
        {
            var ticket = (SupportTicket)sender.Tag;
            ViewModel.SelectedTicket = ticket;
            NoTicketText.Visibility = Visibility.Collapsed;

            SelectedPartnerPhoto.Source = ticket.PartnerPhotoPath != null
                ? new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(ticket.PartnerPhotoPath))
                : null;
        }

        private void Ticket_Collapsed(Expander sender, ExpanderCollapsedEventArgs args)
        {
            ViewModel.SelectedTicket = null;
            SelectedPartnerPhoto.Source = null;
            SelectedPartnerName.Text = string.Empty;
            NoTicketText.Visibility = Visibility.Visible;
        }

        private void Found_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedTicket = (SupportTicket)((Button)sender).Tag;
            ViewModel.ResolveFoundCommand.Execute(null);
            ClearSelectedTicketUI();
        }

        private void NotFound_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedTicket = (SupportTicket)((Button)sender).Tag;
            ViewModel.ResolveNotFoundCommand.Execute(null);
            ClearSelectedTicketUI();
        }

        private void ClearSelectedTicketUI()
        {
            SelectedPartnerPhoto.Source = null;
            SelectedPartnerName.Text = string.Empty;
            NoTicketText.Visibility = Visibility.Visible;
        }

        private void SearchResultsList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Item is DatingProfile profile)
            {
                var container = args.ItemContainer.ContentTemplateRoot as StackPanel;
                if (container == null) return;

                var image = container.Children[0] as Image;
                if (image == null) return;

                var firstPhoto = profile.Photos?.OrderBy(p => p.ProfileOrderIndex).FirstOrDefault();
                if (firstPhoto?.Location != null)
                {
                    image.Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(firstPhoto.Location));
                }
            }
        }
    }
}