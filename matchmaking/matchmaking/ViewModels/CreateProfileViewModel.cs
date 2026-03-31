using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using matchmaking.Services;
using matchmaking.Utils;
using matchmaking.Domain;


namespace matchmaking.ViewModels
{
    internal class CreateProfileViewModel : INotifyPropertyChanged
    {
        private readonly ProfileService _profileService;
        private readonly PhotoService _photoService;
        private readonly MockUserUtil _userUtil;

        private int _currentStep;
        private ProfileData _profileData;
        private bool _termsAccepted;
        private int _currentPhotoIndex;
        private int _userId;
        private string _username;
        private DateTime _birthDate;

        public event PropertyChangedEventHandler? PropertyChanged;

        public CreateProfileViewModel(int userId, ProfileService profileService, PhotoService photoService, MockUserUtil userUtil)
        {
            _profileService = profileService;
            _photoService = photoService;
            _userUtil = userUtil;

            _currentStep = 1;
            _profileData = null;
            _termsAccepted = false;
            _currentPhotoIndex = 0;
            _userId = userId;
        }

        public int CurrentStep
        {
            get { return _currentStep; }
            private set
            {
                if (_currentStep != value)
                {
                    _currentStep = value;
                    OnPropertyChanged(nameof(CurrentStep));
                }
            }
        }

        public ProfileData? ProfileData
        {
            get { return _profileData; }
            private set
            {
                if (_profileData != value)
                {
                    _profileData = value;
                    OnPropertyChanged(nameof(ProfileData));
                }
            }
        }

        public bool TermsAccepted
        {
            get { return _termsAccepted; }
            set
            {
                if (_termsAccepted != value)
                {
                    _termsAccepted = value;
                    OnPropertyChanged(nameof(TermsAccepted));
                }
            }
        }

        public int CurrentPhotoIndex
        {
            get { return _currentPhotoIndex; }
            private set
            {
                if (_currentPhotoIndex != value)
                {
                    _currentPhotoIndex = value;
                    OnPropertyChanged(nameof(CurrentPhotoIndex));
                }
            }
        }

        public int UserId
        {
            get { return _userId; }
            private set
            {
                if (_userId != value)
                {
                    _userId = value;
                    OnPropertyChanged(nameof(UserId));
                }
            }
        }

        public void NextStep()
        {
            if (_currentStep == 4)
            {
                throw new InvalidOperationException("Can't advance past the 4th step!");
            }
            _currentStep++;
        }

        public void PreviousStep()
        {
            if (_currentStep == 1)
            {
                throw new InvalidOperationException("Can't go back before the 1st step!");
            }
            _currentStep--;
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
            _photoService.AddPhoto(photo);
            OnPropertyChanged(nameof(ProfileData));
        }

        public void RemovePhoto(int photoId)
        {
            _photoService.DeleteById(photoId);
            OnPropertyChanged(nameof(ProfileData));
        }

        public void AddInterest(string interest)
        {
            if (_profileData.Interests.Count == 15)
            {
                throw new InvalidOperationException("You can't have more than 15 interests!");
            }

            if (_profileData.Interests.Contains(interest))
            {
                throw new InvalidOperationException("You can't add the same interest twice!");
            }

            _profileData.Interests.Add(interest);
            OnPropertyChanged(nameof(ProfileData));
        }

        public void RemoveInterest(string interest)
        {
            if (_profileData.Interests.Count == 3)
            {
                throw new InvalidOperationException("You can't have less than 3 interests!");
            }

            if (!_profileData.Interests.Contains(interest))
            {
                throw new InvalidOperationException("This interest doesn't exist in your list of interests!");
            }

            _profileData.Interests.Remove(interest);
            OnPropertyChanged(nameof(ProfileData));
        }

        public DatingProfile GetPreviewProfile()
        {
            int age = DateTime.Now.Year - _birthDate.Year;
            if (DateTime.Now < _birthDate.AddYears(age))
            {
                age--;
            }
                

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
            if (_profileData.Photos.Count == 0)
            {
                return;
            }

            CurrentPhotoIndex = (_currentPhotoIndex + 1) % _profileData.Photos.Count;
        }

        public void PreviousPhoto()
        {
            if (_profileData.Photos.Count == 0)
            {
                return;
            }

            if (_currentPhotoIndex == 0)
            {
                CurrentPhotoIndex = _profileData.Photos.Count - 1;
            }
            else
            {
                CurrentPhotoIndex = _currentPhotoIndex - 1;
            }
        }

        public DatingProfile CreateDatingProfile()
        {
            if (!_termsAccepted)
            {
                throw new InvalidOperationException("You didn't accept the terms & conditions!");
            }
            return _profileService.CreateProfile(_userId, _profileData);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
