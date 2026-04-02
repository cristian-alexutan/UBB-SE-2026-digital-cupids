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
            Bindings.Update();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Focus(FocusState.Programmatic);
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape && ViewModel?.IsGuideVisible == true)
            {
                ViewModel.CloseGuideCommand.Execute(null);
                e.Handled = true;
            }
        }
    }
}
