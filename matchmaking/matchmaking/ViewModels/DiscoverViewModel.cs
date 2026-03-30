using matchmaking.Domain;
using matchmaking.Services;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace matchmaking.ViewModels
{
    internal class DiscoverViewModel : INotifyPropertyChanged
    {
        private readonly int _userId;

        private readonly DiscoverService _discoverService;
        private readonly RegisterInteractionUseCase _registerInteractionUseCase;

        private int _currentIndex;
        private int _currentPhotoIndex;
        private List<DatingProfile> _candidates;

        private bool _isGuideVisible;
        private bool _isMatchPopupVisible;
        private string _statusMessage;
        private string _commonCommunitiesText;
        private string _matchPopupMessage;

        public event PropertyChangedEventHandler? PropertyChanged;

        public DiscoverViewModel(int userId, DiscoverService discoverService, RegisterInteractionUseCase registerInteractionUseCase, bool firstLoad = false)
        {
            _userId = userId;
            _discoverService = discoverService;
            _registerInteractionUseCase = registerInteractionUseCase;

            _currentIndex = 0;
            _currentPhotoIndex = 0;
            _candidates = new List<DatingProfile>();

            _isGuideVisible = firstLoad;
            _isMatchPopupVisible = false;
            _statusMessage = string.Empty;
            _commonCommunitiesText = "No communities in common";
            _matchPopupMessage = string.Empty;

            LoadCandidates();
        }

        public bool HasCandidates
        {
            get
            {
                return _candidates.Count > 0 && _currentIndex >= 0 && _currentIndex < _candidates.Count;
            }
        }

        public DatingProfile? CurrentValidCandidate
        {
            get
            {
                if (!HasCandidates)
                {
                    return null;
                }
                return _candidates[_currentIndex];
            }
        }

        public Photo? CurrentPhoto
        {
            get
            {
                DatingProfile? candidate = CurrentValidCandidate;
                if (candidate == null || candidate.Photos == null || candidate.Photos.Count == 0)
                {
                    return null;
                }

                List<Photo> photos = new List<Photo>(candidate.Photos);
                photos.Sort((a, b) => a.ProfileOrderIndex.CompareTo(b.ProfileOrderIndex));

                int index = _currentPhotoIndex;

                if (index < 0 || index >= photos.Count)
                {
                    index = 0;
                }

                return photos[index];
            }
        }

        public int CurrentPhotoIndex
        {
            get
            {
                return _currentPhotoIndex;
            }
            private set
            {
                if (_currentPhotoIndex != value)
                {
                    _currentPhotoIndex = value;
                    OnPropertyChanged(nameof(CurrentPhotoIndex));
                    OnPropertyChanged(nameof(CurrentPhoto));
                }
            }
        }

        public bool IsGuideVisible
        {
            get
            {
                return _isGuideVisible;
            }
            private set
            {
                if (_isGuideVisible != value)
                {
                    _isGuideVisible = value;
                    OnPropertyChanged(nameof(IsGuideVisible));
                }
            }
        }

        public bool IsMatchPopupVisible
        {
            get
            {
                return _isMatchPopupVisible;
            }
            private set
            {
                if (_isMatchPopupVisible != value)
                {
                    _isMatchPopupVisible = value;
                    OnPropertyChanged(nameof(IsMatchPopupVisible));
                }
            }
        }

        public string? CurrentCandidateStarSign
        {
            get
            {
                var candidate = CurrentValidCandidate;
                if (candidate == null || !candidate.DisplayStarSign)
                    return null;
                return candidate.GetStarSign().ToString();
            }
        }

        public Visibility CurrentCandidateStarSignVisibility
        {
            get
            {
                return string.IsNullOrEmpty(CurrentCandidateStarSign)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        public string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            private set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        public string CommonCommunitiesText
        {
            get
            {
                return _commonCommunitiesText;
            }
            private set
            {
                if (_commonCommunitiesText != value)
                {
                    _commonCommunitiesText = value;
                    OnPropertyChanged(nameof(CommonCommunitiesText));
                }
            }
        }

        public string MatchPopupMessage
        {
            get
            {
                return _matchPopupMessage;
            }
            private set
            {
                if (_matchPopupMessage != value)
                {
                    _matchPopupMessage = value;
                    OnPropertyChanged(nameof(MatchPopupMessage));
                }
            }
        }

        public void LoadCandidates()
        {
            _candidates = _discoverService.GetCandidates(_userId);
            _currentIndex = 0;
            CurrentPhotoIndex = 0;

            if (_candidates.Count == 0)
            {
                StatusMessage = "No more profiles to discover";
                CommonCommunitiesText = "No communities in common";
            }
            else
            {
                StatusMessage = string.Empty;
                UpdateCommonCommunitiesText();
            }

            NotifyCandidateChanged();
        }

        public DatingProfile? GetCurrentCandidate()
        {
            if (!HasCandidates)
            {
                StatusMessage = "No more profiles to discover";
                return null;
            }

            return _candidates[_currentIndex];
        }

        public void LikeCurrent()
        {
            if (!HasCandidates)
            {
                return;
            }

            DatingProfile? currentCandidate = GetCurrentCandidate();
            if (currentCandidate == null)
            {
                return;
            }

            Interaction interaction = new Interaction(_userId, currentCandidate.UserId, InteractionType.LIKE);
            bool isMatch = _registerInteractionUseCase.RegisterInteraction(interaction);

            if (isMatch)
            {
                MatchPopupMessage = $"It's a match! You and {currentCandidate.Name} liked each other.";
                IsMatchPopupVisible = true;
            }
            else
            {
                NextCandidate();
            }
        }

        public void SuperLikeCurrent()
        {
            if (!HasCandidates)
            {
                return;
            }

            DatingProfile? currentCandidate = GetCurrentCandidate();
            if (currentCandidate == null)
            {
                return;
            }

            Interaction interaction = new Interaction(_userId, currentCandidate.UserId, InteractionType.SUPER_LIKE);
            bool isMatch = _registerInteractionUseCase.RegisterInteraction(interaction);

            if (isMatch)
            {
                MatchPopupMessage = $"It's a match! You and {currentCandidate.Name} liked each other.";
                IsMatchPopupVisible = true;
            }
            else
            {
                NextCandidate();
            }
        }

        public void PassCurrent()
        {
            if (!HasCandidates)
            {
                return;
            }

            DatingProfile? currentCandidate = GetCurrentCandidate();
            if (currentCandidate == null)
            {
                return;
            }

            Interaction interaction = new Interaction(_userId, currentCandidate.UserId, InteractionType.PASS);
            _registerInteractionUseCase.RegisterInteraction(interaction);

            NextCandidate();
        }

        public void NextCandidate()
        {
            if (!HasCandidates)
            {
                return;
            }

            _candidates.RemoveAt(_currentIndex);

            if (_candidates.Count == 0)
            {
                _currentIndex = 0;
                CurrentPhotoIndex = 0;
                StatusMessage = "No more profiles to discover";
                CommonCommunitiesText = "No communities in common";
            }
            else
            {
                if (_currentIndex >= _candidates.Count)
                {
                    _currentIndex = 0;
                }

                CurrentPhotoIndex = 0;
                StatusMessage = string.Empty;
                UpdateCommonCommunitiesText();
            }

            NotifyCandidateChanged();
        }

        public void NextPhoto()
        {
            DatingProfile? currentCandidate = CurrentValidCandidate;
            if (currentCandidate == null || currentCandidate.Photos == null || currentCandidate.Photos.Count == 0)
            {
                return;
            }

            CurrentPhotoIndex = (CurrentPhotoIndex + 1) % currentCandidate.Photos.Count;
        }

        public void PreviousPhoto()
        {
            DatingProfile? currentCandidate = CurrentValidCandidate;
            if (currentCandidate == null || currentCandidate.Photos == null || currentCandidate.Photos.Count == 0)
            {
                return;
            }

            if (CurrentPhotoIndex == 0)
            {
                CurrentPhotoIndex = currentCandidate.Photos.Count - 1;
            }
            else
            {
                CurrentPhotoIndex--;
            }
        }

        public void OpenGuide()
        {
            IsGuideVisible = true;
        }

        public void CloseGuide()
        {
            IsGuideVisible = false;
        }

        public void CloseMatchPopup()
        {
            IsMatchPopupVisible = false;
            NextCandidate();
        }

        private void UpdateCommonCommunitiesText()
        {
            DatingProfile? candidate = CurrentValidCandidate;
            if (candidate == null)
            {
                CommonCommunitiesText = "No communities in common";
                return;
            }

            List<string> sharedCommunities = _discoverService.GetSharedCommunities(_userId, candidate.UserId);
            if (sharedCommunities.Count == 0)
            {
                CommonCommunitiesText = "No communities in common";
            }
            else
            {
                CommonCommunitiesText = string.Join(", ", sharedCommunities);
            }
        }

        private void NotifyCandidateChanged()
        {
            OnPropertyChanged(nameof(HasCandidates));
            OnPropertyChanged(nameof(CurrentValidCandidate));
            OnPropertyChanged(nameof(CurrentPhoto));
            OnPropertyChanged(nameof(CurrentCandidateStarSign));
            OnPropertyChanged(nameof(CurrentCandidateStarSignVisibility));
            OnPropertyChanged(nameof(CurrentPhotoIndex));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
