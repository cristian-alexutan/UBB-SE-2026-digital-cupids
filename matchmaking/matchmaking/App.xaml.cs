using matchmaking.Domain;
using matchmaking.Repositories;
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

            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();

            var rootFrame = new Frame();
            _window.Content = rootFrame;

            var connectionString = ConnectionString;
            var profileRepo = new ProfileRepository(connectionString);
            var photoRepo = new PhotoRepository(connectionString);
            var bidRepo = new Repositories.BidRepository(connectionString);
            var interactionRepo = new Repositories.InteractionRepository(connectionString);
            var matchRepo = new Repositories.MatchRepository(connectionString);
            var notificationRepo = new Repositories.NotificationRepository(connectionString);

            var userUtil = new Utils.MockUserUtil();
            var profileService = new Services.ProfileService(profileRepo, userUtil);
            var bidService = new Services.BidService(bidRepo);
            var interactionService = new Services.InteractionService(interactionRepo);
            var matchService = new Services.MatchService(matchRepo);
            var notificationService = new Services.NotificationService(notificationRepo);
            var registerInteractionUseCase = new Services.RegisterInteractionUseCase(
                interactionService, matchService, notificationService, profileRepo
            );

            int testUserId = 2;

            try
            {
                var allProfiles = profileService.GetAllProfiles();
                System.Diagnostics.Debug.WriteLine($"[HotSeat Init] Total profiles found: {allProfiles.Count}");

                foreach (var p in allProfiles)
                {
                    System.Diagnostics.Debug.WriteLine($"[HotSeat Init] Profile - UserId: {p.UserId}, Name: {p.Name}, IsArchived: {p.IsArchived}, IsHotSeat: {p.IsHotSeat}, PhotoCount: {p.Photos?.Count ?? 0}");
                }

                if (allProfiles.Count > 0)
                {
                    profileService.ResetHotSeat();
                    System.Diagnostics.Debug.WriteLine("[HotSeat Init] Reset all hot seats");

                    var profileToHotSeat = allProfiles.FirstOrDefault(p => !p.IsArchived && p.UserId != testUserId);
                    if (profileToHotSeat != null)
                    {
                        profileService.SetHotSeat(profileToHotSeat.UserId);
                        System.Diagnostics.Debug.WriteLine($"[HotSeat Init] Set HotSeat profile: UserId={profileToHotSeat.UserId}, Name={profileToHotSeat.Name}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[HotSeat Init] No suitable profile found for HotSeat (all archived or testUserId match)");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[HotSeat Init] No profiles found in database");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[HotSeat Init ERROR] {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[HotSeat Init ERROR] StackTrace: {ex.StackTrace}");
            }

            var view = new Views.HotSeatView();
            var viewModel = new ViewModels.HotSeatViewModel(
                testUserId, profileService, bidService, registerInteractionUseCase
            );

            view.SetViewModel(viewModel);
            rootFrame.Content = view;
        }
    }
}
