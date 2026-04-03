using matchmaking.Domain;
using matchmaking.Repositories;
using matchmaking.Services;
using matchmaking.Utils;
using matchmaking.ViewModels;
using matchmaking.Views;
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

        public static string Email { get; private set; }

        public static string Password { get; private set; }

        public static int UserID { get; private set; }

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
            //    },
            //    "Email": {
            //        "Address": "[email address]",
            //        "Password": "[email password]"
            //    },
            //    "UserId": "[user id]"
            //}

            // apoi, click dreapta pe appsettings.Development.json, Properties, si setati "Copy to Output Directory" la "Copy if newer"

            ConnectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
            Email = config["Email:Address"] ?? string.Empty;
            Password = config["Email:Password"] ?? string.Empty;
            UserID = int.Parse(config["UserId"] ?? "0");

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
            _window.Activate();
        }
    }
}
