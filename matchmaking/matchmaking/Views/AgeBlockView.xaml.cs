using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace matchmaking.Views
{
    internal sealed partial class AgeBlockView : Page
    {
        public AgeBlockView()
        {
            InitializeComponent();
        }

        private void HandleExitAppClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}