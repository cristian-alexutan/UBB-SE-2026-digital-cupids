using matchmaking.Domain;
using matchmaking.Repositories;
using matchmaking.Views;
using matchmaking.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace matchmaking
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public Window? _window;

        public static Window MainWindow { get; private set; }

        public static string ConnectionString { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();

            // va creati, in fisierul proiectului, appsettings.Development.json, cu urmatorul continut:

            //{
            //    "ConnectionStrings": {
            //        "DefaultConnection": "[connection string]"
            //    }
            //}

            // apoi, click dreapta pe appsettings.Development.json, Properties, si setati "Copy to Output Directory" la "Copy if newer"

            ConnectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;

            Debug.WriteLine($"Connection string loaded: {ConnectionString}");

            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            MainWindow = _window;
        }
    }
}
