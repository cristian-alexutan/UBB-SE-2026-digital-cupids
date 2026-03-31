using matchmaking.Domain;
using matchmaking.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace matchmaking.ViewModels
{
    internal class HotSeatViewModel : INotifyPropertyChanged
    {
        private ProfileService _profileService;
        private BidService _bidService;
        private RegisterInteractionUseCase _registerInteractionUseCase;

        private int _userId;
        private DatingProfile _hotSeatProfile;
        private int _highestBid;
        private string _errorMessage;
        private int _bidInput;
        private int _currentPhotoIndex;


        public bool ShowInteractionButtons =>
        HotSeatProfile != null && _userId!=_hotSeatProfile.UserId;

        public string? CurrentPhoto =>
        HotSeatProfile?.Photos != null && HotSeatProfile.Photos.Count > 0
        ? HotSeatProfile.Photos[CurrentPhotoIndex].Location
        : null;

        public bool HasHotSeatProfile => HotSeatProfile != null;
        public bool HasNoBid => HighestBid == 0;
        public bool HasBid => HighestBid > 0;
        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public DatingProfile HotSeatProfile
        {
            get => _hotSeatProfile;
            private set { _hotSeatProfile = value; OnPropertyChanged();
                OnPropertyChanged(nameof(ShowInteractionButtons));
                OnPropertyChanged(nameof(CurrentPhoto));
                OnPropertyChanged(nameof(HasHotSeatProfile));
            }
        }

        public int HighestBid
        {
            get => _highestBid;
            private set { _highestBid = value; OnPropertyChanged();
                OnPropertyChanged(nameof(HasNoBid));
                OnPropertyChanged(nameof(HasBid));
            }
        }
        public string ErrorMessage
        {
            get => _errorMessage;
            private set { _errorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasErrorMessage)); }
        }

        public int BidInput
        {
            get => _bidInput;
            set { _bidInput = value; OnPropertyChanged(); }
        }

        public int CurrentPhotoIndex
        {
            get => _currentPhotoIndex;
            private set { _currentPhotoIndex = value; OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentPhoto));
            }
        }

        public HotSeatViewModel(int userId, ProfileService profileService, BidService bidService, RegisterInteractionUseCase registerInteractionUseCase)
        {
            _userId = userId;
            _profileService = profileService;
            _bidService = bidService;
            _registerInteractionUseCase = registerInteractionUseCase;
        }

        public void LoadHotSeat()
        {
            ErrorMessage = string.Empty;
            CurrentPhotoIndex = 0;

            var allProfiles = _profileService.GetAllProfiles();

            System.Diagnostics.Debug.WriteLine("=== ALL PROFILES IN DATABASE ===");
            foreach (var profile in allProfiles)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadHotSeat] UserId: {profile.UserId}, Name: {profile.Name}, IsHotSeat: {profile.IsHotSeat}, IsArchived: {profile.IsArchived}, PhotoCount: {profile.Photos?.Count ?? 0}");
            }
            System.Diagnostics.Debug.WriteLine("=== END OF PROFILES ===");

           
            HotSeatProfile = allProfiles.FirstOrDefault(p => p.IsHotSeat && !p.IsArchived);

            if (HotSeatProfile != null)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadHotSeat]  FOUND HOT SEAT PROFILE");
                System.Diagnostics.Debug.WriteLine($"[LoadHotSeat]   UserId: {HotSeatProfile.UserId}");
                System.Diagnostics.Debug.WriteLine($"[LoadHotSeat]   Name: {HotSeatProfile.Name}");
                System.Diagnostics.Debug.WriteLine($"[LoadHotSeat]   PhotoCount: {HotSeatProfile.Photos?.Count ?? 0}");
                System.Diagnostics.Debug.WriteLine($"[LoadHotSeat]   Bio: {HotSeatProfile.Bio}");

                if (HotSeatProfile.Photos == null || HotSeatProfile.Photos.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[LoadHotSeat]   WARNING: No photos for this profile!");
                }
                else
                {
                    foreach (var photo in HotSeatProfile.Photos)
                    {
                        System.Diagnostics.Debug.WriteLine($"[LoadHotSeat]   Photo: {photo.Location}");
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[LoadHotSeat]  NO HOT SEAT PROFILE FOUND!");
            }

            HighestBid = _bidService.getHighestBid();
        }

        public void PlaceBid()
        {
            ErrorMessage = string.Empty;
            try
            {
                Bid newBid = new Bid(_userId, (int)BidInput);
                _bidService.AddBid(newBid);
                HighestBid = _bidService.getHighestBid();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public void BoostProfile()
        {
            ErrorMessage = string.Empty;
            DatingProfile profile = _profileService.GetProfileById(_userId);

            if (profile.IsBoosted && profile.BoostDay == DateTime.Today.Day)
            {
                ErrorMessage = "Your profile is already boosted for today.";    
                return;
            }

            profile.IsBoosted = true;
            profile.BoostDay = DateTime.Today.Day;
            _profileService.UpdateBoost(_userId);
        }

        public void LikeHotSeat()
        {
            if (_userId == HotSeatProfile.UserId)
            {
                throw new Exception("You cant like your own profile");
            }

            Interaction inter = new Interaction(_userId, HotSeatProfile.UserId, InteractionType.LIKE);
            _registerInteractionUseCase.RegisterInteraction(inter);
        }

        public void SuperLikeHotSeat()
        {
            if (_userId == HotSeatProfile.UserId)
            {
                throw new Exception("You cant like your own profile");
            }
            Interaction inter = new Interaction(_userId, HotSeatProfile.UserId, InteractionType.SUPER_LIKE);
            _registerInteractionUseCase.RegisterInteraction(inter);
        }

        public void NextPhoto()
        {
            if (HotSeatProfile.Photos == null || HotSeatProfile.Photos.Count == 0) return;
            CurrentPhotoIndex = (CurrentPhotoIndex + 1 + HotSeatProfile.Photos.Count) % HotSeatProfile.Photos.Count;
        }

        public void PreviousPhoto()
        {
            if (HotSeatProfile?.Photos == null || HotSeatProfile.Photos.Count == 0) return;
            CurrentPhotoIndex = (CurrentPhotoIndex - 1) % HotSeatProfile.Photos.Count;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}