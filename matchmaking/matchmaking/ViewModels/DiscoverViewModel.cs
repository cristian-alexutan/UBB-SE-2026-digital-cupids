using matchmaking.Domain;
using matchmaking.Services;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace matchmaking.ViewModels
{
    internal class DiscoverViewModel : ObservableObject
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

        public event Action<string>? ErrorOccurred;

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

            PassCommand = new RelayCommand(PassCurrent, () => HasCandidates);
            LikeCommand = new RelayCommand(LikeCurrent, () => HasCandidates);
            SuperLikeCommand = new RelayCommand(SuperLikeCurrent, () => HasCandidates);
            NextPhotoCommand = new RelayCommand(NextPhoto, () => HasCandidates);
            PreviousPhotoCommand = new RelayCommand(PreviousPhoto, () => HasCandidates);
            OpenGuideCommand = new RelayCommand(OpenGuide);
            CloseGuideCommand = new RelayCommand(CloseGuide);
            CloseMatchPopupCommand = new RelayCommand(CloseMatchPopup, () => IsMatchPopupVisible);

            LoadCandidates();
        }

        public RelayCommand PassCommand { get; }
        public RelayCommand LikeCommand { get; }
        public RelayCommand SuperLikeCommand { get; }
        public RelayCommand NextPhotoCommand { get; }
        public RelayCommand PreviousPhotoCommand { get; }
        public RelayCommand OpenGuideCommand { get; }
        public RelayCommand CloseGuideCommand { get; }
        public RelayCommand CloseMatchPopupCommand { get; }

        public bool HasCandidates => _candidates.Count > 0 && _currentIndex >= 0 && _currentIndex < _candidates.Count;

        public Visibility CandidatesVisibility => HasCandidates ? Visibility.Visible : Visibility.Collapsed;
        public Visibility NoCandidatesVisibility => HasCandidates ? Visibility.Collapsed : Visibility.Visible;
        public Visibility GuideVisibility => IsGuideVisible ? Visibility.Visible : Visibility.Collapsed;
        public Visibility MatchPopupVisibility => IsMatchPopupVisible ? Visibility.Visible : Visibility.Collapsed;

        public DatingProfile? CurrentValidCandidate => HasCandidates ? _candidates[_currentIndex] : null;

        public Photo? CurrentPhoto
        {
            get
            {
                DatingProfile? candidate = CurrentValidCandidate;
                if (candidate == null || candidate.Photos == null || candidate.Photos.Count == 0)
                {
                    return null;
                }

                List<Photo> photos = candidate.Photos.OrderBy(p => p.ProfileOrderIndex).ToList();
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
            get => _currentPhotoIndex;
            private set
            {
                if (SetProperty(ref _currentPhotoIndex, value))
                {
                    OnPropertyChanged(nameof(CurrentPhoto));
                }
            }
        }

        public bool IsGuideVisible
        {
            get => _isGuideVisible;
            private set
            {
                if (SetProperty(ref _isGuideVisible, value))
                {
                    OnPropertyChanged(nameof(GuideVisibility));
                }
            }
        }

        public bool IsMatchPopupVisible
        {
            get => _isMatchPopupVisible;
            private set
            {
                if (SetProperty(ref _isMatchPopupVisible, value))
                {
                    OnPropertyChanged(nameof(MatchPopupVisibility));
                    UpdateCommandStates();
                }
            }
        }

        public string? CurrentCandidateStarSign
        {
            get
            {
                DatingProfile? candidate = CurrentValidCandidate;
                if (candidate == null || !candidate.DisplayStarSign)
                {
                    return null;
                }

                return candidate.GetStarSign().ToString();
            }
        }

        public string? CurrentCandidateLoverType
        {
            get
            {
                DatingProfile? candidate = CurrentValidCandidate;
                if (candidate == null || !candidate.LoverType.HasValue)
                {
                    return null;
                }

                return candidate.LoverType.Value.ToString().Replace("_", " ");
            }
        }

        public Visibility CurrentCandidateLoverTypeVisibility => string.IsNullOrEmpty(CurrentCandidateLoverType)
            ? Visibility.Collapsed
            : Visibility.Visible;

        public Visibility CurrentCandidateStarSignVisibility => string.IsNullOrEmpty(CurrentCandidateStarSign)
            ? Visibility.Collapsed
            : Visibility.Visible;

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public string CommonCommunitiesText
        {
            get => _commonCommunitiesText;
            private set
            {
                if (SetProperty(ref _commonCommunitiesText, value))
                {
                    OnPropertyChanged(nameof(CommonCommunitiesVisibility));
                }
            }
        }

        public Visibility CommonCommunitiesVisibility => string.IsNullOrWhiteSpace(CommonCommunitiesText)
            ? Visibility.Collapsed
            : Visibility.Visible;

        public string MatchPopupMessage
        {
            get => _matchPopupMessage;
            private set => SetProperty(ref _matchPopupMessage, value);
        }

        public void LoadCandidates()
        {
            try
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
            catch (Exception ex)
            {
                _candidates = new List<DatingProfile>();
                _currentIndex = 0;
                CurrentPhotoIndex = 0;
                StatusMessage = "Could not load discover profiles.";
                CommonCommunitiesText = "No communities in common";
                NotifyCandidateChanged();
                ReportError("Could not load discover profiles", ex);
            }
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

            try
            {
                DatingProfile? currentCandidate = GetCurrentCandidate();
                if (currentCandidate == null)
                {
                    return;
                }

                Interaction interaction = new Interaction(_userId, currentCandidate.UserId, InteractionType.LIKE);
                bool isMatch = _registerInteractionUseCase.RegisterInteraction(interaction);

                if (isMatch)
                {
                    MatchPopupMessage = $"You and {currentCandidate.Name} liked each other.";
                    IsMatchPopupVisible = true;
                }
                else
                {
                    NextCandidate();
                }
            }
            catch (Exception ex)
            {
                ReportError("Could not register Like interaction", ex);
            }
        }

        public void SuperLikeCurrent()
        {
            if (!HasCandidates)
            {
                return;
            }

            try
            {
                DatingProfile? currentCandidate = GetCurrentCandidate();
                if (currentCandidate == null)
                {
                    return;
                }

                Interaction interaction = new Interaction(_userId, currentCandidate.UserId, InteractionType.SUPER_LIKE);
                bool isMatch = _registerInteractionUseCase.RegisterInteraction(interaction);

                if (isMatch)
                {
                    MatchPopupMessage = $"You and {currentCandidate.Name} liked each other.";
                    IsMatchPopupVisible = true;
                }
                else
                {
                    NextCandidate();
                }
            }
            catch (Exception ex)
            {
                ReportError("Could not register Super-Like interaction", ex);
            }
        }

        public void PassCurrent()
        {
            if (!HasCandidates)
            {
                return;
            }

            try
            {
                DatingProfile? currentCandidate = GetCurrentCandidate();
                if (currentCandidate == null)
                {
                    return;
                }

                Interaction interaction = new Interaction(_userId, currentCandidate.UserId, InteractionType.PASS);
                _registerInteractionUseCase.RegisterInteraction(interaction);
                NextCandidate();
            }
            catch (Exception ex)
            {
                ReportError("Could not register Pass interaction", ex);
            }
        }

        public void NextCandidate()
        {
            if (!HasCandidates)
            {
                return;
            }

            try
            {
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
            catch (Exception ex)
            {
                ReportError("Could not move to next candidate", ex);
            }
        }

        public void NextPhoto()
        {
            try
            {
                DatingProfile? currentCandidate = CurrentValidCandidate;
                if (currentCandidate == null || currentCandidate.Photos == null || currentCandidate.Photos.Count == 0)
                {
                    return;
                }

                CurrentPhotoIndex = (CurrentPhotoIndex + 1) % currentCandidate.Photos.Count;
            }
            catch (Exception ex)
            {
                ReportError("Could not move to next photo", ex);
            }
        }

        public void PreviousPhoto()
        {
            try
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
            catch (Exception ex)
            {
                ReportError("Could not move to previous photo", ex);
            }
        }

        public void OpenGuide()
        {
            try
            {
                IsGuideVisible = true;
            }
            catch (Exception ex)
            {
                ReportError("Could not open guide", ex);
            }
        }

        public void CloseGuide()
        {
            try
            {
                IsGuideVisible = false;
            }
            catch (Exception ex)
            {
                ReportError("Could not close guide", ex);
            }
        }

        public void CloseMatchPopup()
        {
            try
            {
                IsMatchPopupVisible = false;
                NextCandidate();
            }
            catch (Exception ex)
            {
                ReportError("Could not close match popup", ex);
            }
        }

        private void UpdateCommonCommunitiesText()
        {
            try
            {
                DatingProfile? candidate = CurrentValidCandidate;
                if (candidate == null)
                {
                    CommonCommunitiesText = "No communities in common";
                    return;
                }

                List<string> sharedCommunities = _discoverService.GetSharedCommunities(_userId, candidate.UserId);
                CommonCommunitiesText = sharedCommunities.Count == 0
                    ? "No communities in common"
                    : string.Join(", ", sharedCommunities);
            }
            catch (Exception ex)
            {
                CommonCommunitiesText = "No communities in common";
                ReportError("Could not load shared communities", ex);
            }
        }

        private void NotifyCandidateChanged()
        {
            OnPropertyChanged(nameof(HasCandidates));
            OnPropertyChanged(nameof(CandidatesVisibility));
            OnPropertyChanged(nameof(NoCandidatesVisibility));
            OnPropertyChanged(nameof(CurrentValidCandidate));
            OnPropertyChanged(nameof(CurrentPhoto));
            OnPropertyChanged(nameof(CurrentCandidateStarSign));
            OnPropertyChanged(nameof(CurrentCandidateStarSignVisibility));
            OnPropertyChanged(nameof(CurrentCandidateLoverType));
            OnPropertyChanged(nameof(CurrentCandidateLoverTypeVisibility));
            OnPropertyChanged(nameof(CurrentPhotoIndex));
            UpdateCommandStates();
        }

        private void UpdateCommandStates()
        {
            PassCommand.NotifyCanExecuteChanged();
            LikeCommand.NotifyCanExecuteChanged();
            SuperLikeCommand.NotifyCanExecuteChanged();
            NextPhotoCommand.NotifyCanExecuteChanged();
            PreviousPhotoCommand.NotifyCanExecuteChanged();
            CloseMatchPopupCommand.NotifyCanExecuteChanged();
        }

        private void ReportError(string message, Exception ex)
        {
            ErrorOccurred?.Invoke($"{message}: {ex.Message}");
        }
    }
}
