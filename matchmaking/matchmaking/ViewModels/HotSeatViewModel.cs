using matchmaking.Domain;
using matchmaking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace matchmaking.ViewModels
{
    internal class HotSeatViewModel : ObservableObject
    {
        private ProfileService _profileService;
        private BidService _bidService;
        private RegisterInteractionUseCase _registerInteractionUseCase;

        private int _userId;
        private DatingProfile _hotSeatProfile;
        private int _highestBid;
        private string _errorMessage = string.Empty;
        private double _bidInput;
        private int _currentPhotoIndex;

        public string HighestBidDisplay => HighestBid.ToString();
        public string AgeDisplay => HotSeatProfile?.Age.ToString() ?? string.Empty;
        public string GenderDisplay => HotSeatProfile?.Gender.ToString() ?? string.Empty;
        public string LoverTypeDisplay => HotSeatProfile?.LoverType?.ToString() ?? string.Empty;

        public string? StarSignDisplay =>
            HotSeatProfile != null && HotSeatProfile.DisplayStarSign
            ? HotSeatProfile.GetStarSign().ToString()
            : string.Empty;

        public string? CurrentPhoto =>
            HotSeatProfile?.Photos != null && HotSeatProfile.Photos.Count > 0
            ? HotSeatProfile.Photos[CurrentPhotoIndex].Location
            : null;

        public bool ShowInteractionButtons =>
            HotSeatProfile != null && _userId != _hotSeatProfile.UserId;

        public bool ShowStarSign => HotSeatProfile != null && HotSeatProfile.DisplayStarSign;
        public bool HasHotSeatProfile => HotSeatProfile != null;
        public bool HasNoBid => HighestBid == 0;
        public bool HasBid => HighestBid > 0;
        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public DatingProfile HotSeatProfile
        {
            get => _hotSeatProfile;
            private set
            {
                SetProperty(ref _hotSeatProfile, value);
                OnPropertyChanged(nameof(ShowInteractionButtons));
                OnPropertyChanged(nameof(CurrentPhoto));
                OnPropertyChanged(nameof(HasHotSeatProfile));
                OnPropertyChanged(nameof(StarSignDisplay));
                OnPropertyChanged(nameof(ShowStarSign));
                OnPropertyChanged(nameof(GenderDisplay));
                OnPropertyChanged(nameof(LoverTypeDisplay));
                OnPropertyChanged(nameof(AgeDisplay));
            }
        }

        public int HighestBid
        {
            get => _highestBid;
            private set
            {
                SetProperty(ref _highestBid, value);
                OnPropertyChanged(nameof(HasNoBid));
                OnPropertyChanged(nameof(HasBid));
                OnPropertyChanged(nameof(HighestBidDisplay));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(HasErrorMessage));
            }
        }

        public double BidInput
        {
            get => _bidInput;
            set { SetProperty(ref _bidInput, value); }
        }

        public int CurrentPhotoIndex
        {
            get => _currentPhotoIndex;
            private set
            {
                SetProperty(ref _currentPhotoIndex, value);
                OnPropertyChanged(nameof(CurrentPhoto));
            }
        }

        public ICommand LoadHotSeatCommand { get; }
        public ICommand PlaceBidCommand { get; }
        public ICommand BoostProfileCommand { get; }
        public ICommand LikeHotSeatCommand { get; }
        public ICommand SuperLikeHotSeatCommand { get; }
        public ICommand NextPhotoCommand { get; }
        public ICommand PreviousPhotoCommand { get; }

        public HotSeatViewModel(int userId, ProfileService profileService, BidService bidService, RegisterInteractionUseCase registerInteractionUseCase)
        {
            _userId = userId;
            _profileService = profileService;
            _bidService = bidService;
            _registerInteractionUseCase = registerInteractionUseCase;

            LoadHotSeatCommand = new RelayCommand(LoadHotSeat);
            PlaceBidCommand = new RelayCommand(PlaceBid);
            BoostProfileCommand = new RelayCommand(BoostProfile);
            LikeHotSeatCommand = new RelayCommand(LikeHotSeat, () => HotSeatProfile != null && HotSeatProfile.UserId != _userId);
            SuperLikeHotSeatCommand = new RelayCommand(SuperLikeHotSeat, () => HotSeatProfile != null && HotSeatProfile.UserId != _userId);
            NextPhotoCommand = new RelayCommand(NextPhoto);
            PreviousPhotoCommand = new RelayCommand(PreviousPhoto);
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
                System.Diagnostics.Debug.WriteLine($"[LoadHotSeat] FOUND HOT SEAT PROFILE");
                System.Diagnostics.Debug.WriteLine($"[LoadHotSeat] UserId: {HotSeatProfile.UserId}");
                System.Diagnostics.Debug.WriteLine($"[LoadHotSeat] Name: {HotSeatProfile.Name}");
                System.Diagnostics.Debug.WriteLine($"[LoadHotSeat] PhotoCount: {HotSeatProfile.Photos?.Count ?? 0}");
                System.Diagnostics.Debug.WriteLine($"[LoadHotSeat] Bio: {HotSeatProfile.Bio}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[LoadHotSeat] NO HOT SEAT PROFILE FOUND!");
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
                throw new Exception("You cant like your own profile");

            Interaction inter = new Interaction(_userId, HotSeatProfile.UserId, InteractionType.LIKE);
            _registerInteractionUseCase.RegisterInteraction(inter);
        }

        public void SuperLikeHotSeat()
        {
            if (_userId == HotSeatProfile.UserId)
                throw new Exception("You cant like your own profile");

            Interaction inter = new Interaction(_userId, HotSeatProfile.UserId, InteractionType.SUPER_LIKE);
            _registerInteractionUseCase.RegisterInteraction(inter);
        }

        public void NextPhoto()
        {
            if (HotSeatProfile?.Photos == null || HotSeatProfile.Photos.Count == 0) return;
            CurrentPhotoIndex = (CurrentPhotoIndex + 1 + HotSeatProfile.Photos.Count) % HotSeatProfile.Photos.Count;
        }

        public void PreviousPhoto()
        {
            if (HotSeatProfile?.Photos == null || HotSeatProfile.Photos.Count == 0) return;
            CurrentPhotoIndex = (CurrentPhotoIndex - 1 + HotSeatProfile.Photos.Count) % HotSeatProfile.Photos.Count;
        }
    }
}