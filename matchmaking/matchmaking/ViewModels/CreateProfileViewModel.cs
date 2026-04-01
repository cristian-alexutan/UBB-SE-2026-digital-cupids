using System;
using System.Collections.Generic;
using System.Linq;
using matchmaking.Services;
using matchmaking.Utils;
using matchmaking.Domain;

namespace matchmaking.ViewModels
{
    internal class CreateProfileViewModel : ObservableObject
    {
        private readonly ProfileService _profileService;
        private readonly MockUserUtil _userUtil;

        private int _currentStep;
        private ProfileData? _profileData;
        private bool _termsAccepted;
        private int _currentPhotoIndex;
        private int _userId;
        private string _username = string.Empty;
        private DateTime _birthDate;

        public RelayCommand NextStepCommand { get; }
        public RelayCommand PreviousStepCommand { get; }
        public RelayCommand NextPhotoCommand { get; }
        public RelayCommand PreviousPhotoCommand { get; }
        public RelayCommand CreateProfileCommand { get; }

        public CreateProfileViewModel(int userId, ProfileService profileService, MockUserUtil userUtil)
        {
            _profileService = profileService;
            _userUtil = userUtil;

            _currentStep = 1;
            _profileData = null;
            _termsAccepted = false;
            _currentPhotoIndex = 0;
            _userId = userId;

            NextStepCommand = new RelayCommand(NextStep, () => _currentStep < 4);
            PreviousStepCommand = new RelayCommand(PreviousStep, () => _currentStep > 1);
            NextPhotoCommand = new RelayCommand(NextPhoto, () => _profileData?.Photos.Count > 0);
            PreviousPhotoCommand = new RelayCommand(PreviousPhoto, () => _profileData?.Photos.Count > 0);
            CreateProfileCommand = new RelayCommand(ExecuteCreateProfile, () => _termsAccepted);
        }

        public int CurrentStep
        {
            get => _currentStep;
            private set
            {
                if (SetProperty(ref _currentStep, value))
                {
                    NextStepCommand.NotifyCanExecuteChanged();
                    PreviousStepCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public ProfileData? ProfileData
        {
            get => _profileData;
            private set => SetProperty(ref _profileData, value);
        }

        public bool TermsAccepted
        {
            get => _termsAccepted;
            set
            {
                if (SetProperty(ref _termsAccepted, value))
                {
                    CreateProfileCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public int CurrentPhotoIndex
        {
            get => _currentPhotoIndex;
            private set => SetProperty(ref _currentPhotoIndex, value);
        }

        public int UserId
        {
            get => _userId;
            private set => SetProperty(ref _userId, value);
        }

        public void NextStep()
        {
            if (_currentStep == 4)
                throw new InvalidOperationException("Can't advance past the 4th step!");
            CurrentStep++;
        }

        public void PreviousStep()
        {
            if (_currentStep == 1)
                throw new InvalidOperationException("Can't go back before the 1st step!");
            CurrentStep--;
        }

        public void LoadUserData(int userId)
        {
            UserData userData = _userUtil.GetUserData(userId);
            _username = userData.Username;
            _birthDate = userData.Birthdate;

            _profileData = new ProfileData(
                Gender.OTHER,
                new List<Gender>(),
                string.Empty,
                string.Empty,
                50,
                18,
                99,
                string.Empty,
                false,
                new List<Photo>(),
                new List<string>(),
                null
            );
        }

        public void AddPhoto(Photo photo)
        {
            if (_profileData.Photos.Count >= 6)
                throw new InvalidOperationException("You cannot upload more than 6 photos!");
            photo.ProfileOrderIndex = _profileData.Photos.Count;
            _profileData.Photos.Add(photo);
            OnPropertyChanged(nameof(ProfileData));
            NextPhotoCommand.NotifyCanExecuteChanged();
            PreviousPhotoCommand.NotifyCanExecuteChanged();
        }

        public void RemovePhoto(int photoId)
        {
            var photo = _profileData.Photos.FirstOrDefault(p => p.PhotoId == photoId);
            if (photo == null) return;
            if (_profileData.Photos.Count <= 2)
                throw new InvalidOperationException("You must have at least 2 photos!");

            _profileData.Photos.Remove(photo);
            for (int i = 0; i < _profileData.Photos.Count; i++)
                _profileData.Photos[i].ProfileOrderIndex = i;

            OnPropertyChanged(nameof(ProfileData));
            NextPhotoCommand.NotifyCanExecuteChanged();
            PreviousPhotoCommand.NotifyCanExecuteChanged();
        }

        public void SwapPhotos(int fromIndex, int toIndex)
        {
            bool fromOutOfBounds = fromIndex < 0 || fromIndex >= _profileData.Photos.Count;
            bool toOutOfBounds = toIndex < 0 || toIndex >= _profileData.Photos.Count;
            if (fromOutOfBounds || toOutOfBounds) return;

            Photo temp = _profileData.Photos[fromIndex];
            _profileData.Photos[fromIndex] = _profileData.Photos[toIndex];
            _profileData.Photos[toIndex] = temp;

            for (int i = 0; i < _profileData.Photos.Count; i++)
                _profileData.Photos[i].ProfileOrderIndex = i;

            OnPropertyChanged(nameof(ProfileData));
        }

        public void AddInterest(string interest)
        {
            if (_profileData.Interests.Count >= 15)
                throw new InvalidOperationException("You can't have more than 15 interests!");
            if (_profileData.Interests.Contains(interest))
                throw new InvalidOperationException("You can't add the same interest twice!");
            _profileData.Interests.Add(interest);
            OnPropertyChanged(nameof(ProfileData));
        }

        public void RemoveInterest(string interest)
        {
            if (!_profileData.Interests.Contains(interest))
                throw new InvalidOperationException("This interest doesn't exist in your list!");
            _profileData.Interests.Remove(interest);
            OnPropertyChanged(nameof(ProfileData));
        }

        public DatingProfile GetPreviewProfile()
        {
            int age = DateTime.Now.Year - _birthDate.Year;
            if (DateTime.Now < _birthDate.AddYears(age)) age--;

            return new DatingProfile(
                _username,
                _profileData.Gender,
                _profileData.PreferredGenders,
                _profileData.Location,
                _profileData.Nationality,
                _profileData.MaxDistance,
                age,
                _profileData.MinPreferredAge,
                _profileData.MaxPreferredAge,
                _profileData.Bio,
                _profileData.DisplayStarSign,
                false,
                _profileData.Photos,
                _profileData.Interests,
                _birthDate,
                _profileData.LoverType,
                false,
                false,
                0,
                0
            );
        }

        public void NextPhoto()
        {
            if (_profileData.Photos.Count == 0) return;
            CurrentPhotoIndex = (_currentPhotoIndex + 1) % _profileData.Photos.Count;
        }

        public void PreviousPhoto()
        {
            if (_profileData.Photos.Count == 0) return;
            CurrentPhotoIndex = _currentPhotoIndex == 0
                ? _profileData.Photos.Count - 1
                : _currentPhotoIndex - 1;
        }

        public DatingProfile CreateDatingProfile()
        {
            if (!_termsAccepted)
                throw new InvalidOperationException("You didn't accept the terms & conditions!");
            return _profileService.CreateProfile(_userId, _profileData);
        }

        private void ExecuteCreateProfile() => CreateDatingProfile();
    }
}