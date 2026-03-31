using matchmaking.Domain;
using matchmaking.Services;
using matchmaking.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace matchmaking.ViewModels
{
    internal class EditProfileViewModel : INotifyPropertyChanged
    {
        public int _userId { get;  }
        private readonly ProfileService _profileService;
        private readonly PhotoService _photoService;
        private readonly QuestionaireUtil _questionaireUtil;
        private readonly InterestUtil _interestUtil;

        public event PropertyChangedEventHandler? PropertyChanged;
        public string Name { get; private set; } = string.Empty;
        public int Age { get; private set; }
        public StarSign StarSign { get; private set; }
        public LoverType? LoverType { get; private set; }


        private string _bio = string.Empty;
        private string _location = string.Empty;
        private string _nationality = string.Empty;
        private int _maxDistance;
        private int _minPreferredAge;
        private int _maxPreferredAge;
        private Gender _gender;
        private bool _displayStarSign;
        private bool _isArchived;
        private List<Gender> _preferredGenders = new();
        private ObservableCollection<Photo> _photos = new();
        private ObservableCollection<string> _interests = new();

        private string _errorMessage = string.Empty;

        private ObservableCollection<string> _shuffledQuestions = new();
        private ObservableCollection<int> _answers = new();

        public string Bio
        {
            get => _bio;
            set { if (_bio != value) { _bio = value; OnPropertyChanged(nameof(Bio)); } }
        }

        public string Location
        {
            get => _location;
            set { if (_location != value) { _location = value; OnPropertyChanged(nameof(Location)); } }
        }

        public string Nationality
        {
            get => _nationality;
            set { if (_nationality != value) { _nationality = value; OnPropertyChanged(nameof(Nationality)); } }
        }

        public int MaxDistance
        {
            get => _maxDistance;
            set { if (_maxDistance != value) { _maxDistance = value; OnPropertyChanged(nameof(MaxDistance)); } }
        }

        public int MinPreferredAge
        {
            get => _minPreferredAge;
            set { if (_minPreferredAge != value) { _minPreferredAge = value; OnPropertyChanged(nameof(MinPreferredAge)); } }
        }

        public int MaxPreferredAge
        {
            get => _maxPreferredAge;
            set { if (_maxPreferredAge != value) { _maxPreferredAge = value; OnPropertyChanged(nameof(MaxPreferredAge)); } }
        }

        public Gender Gender
        {
            get => _gender;
            set { if (_gender != value) { _gender = value; OnPropertyChanged(nameof(Gender)); } }
        }
        public List<string> GenderOptions { get; } = new List<string>{"Male", "Female", "Non-Binary", "Other"};


        public string SelectedGender
        {
            get => _gender switch
            {
                Gender.MALE => "Male",
                Gender.FEMALE => "Female",
                Gender.NON_BINARY => "Non-Binary",
                Gender.OTHER => "Other",
                _ => "Male"
            };
            set
            {
                Gender newGender = value switch
                {
                    "Female" => Gender.FEMALE,
                    "Non-Binary" => Gender.NON_BINARY,
                    "Other" => Gender.OTHER,
                    _ => Gender.MALE
                };
                if (_gender != newGender)
                {
                    _gender = newGender;
                    OnPropertyChanged(nameof(Gender));
                    OnPropertyChanged(nameof(SelectedGender));
                }
            }
        }
        public bool DisplayStarSign
        {
            get => _displayStarSign;
            set { if (_displayStarSign != value) { _displayStarSign = value; OnPropertyChanged(nameof(DisplayStarSign)); } }
        }

        public bool IsArchived
        {
            get => _isArchived;
            set 
            { 
                if (_isArchived != value) 
                { 
                    _isArchived = value; 
                    OnPropertyChanged(nameof(IsArchived)); 
                    OnPropertyChanged(nameof(IsNotArchived)); 
                } 
            }
        }

        public bool IsNotArchived => !IsArchived;

        public List<Gender> PreferredGenders
        {
            get => _preferredGenders;
            set { if (_preferredGenders != value) { _preferredGenders = value; OnPropertyChanged(nameof(PreferredGenders)); } }
        }

        public List<Photo> Photos
        {
            get => _photos.ToList();
            private set { if (!_photos.SequenceEqual(value ?? new())) { _photos = new(value ?? new()); OnPropertyChanged(nameof(Photos)); } }
        }

        public List<string> Interests
        {
            get => _interests.ToList();
            private set { if (!_interests.SequenceEqual(value ?? new())) { _interests = new(value ?? new()); OnPropertyChanged(nameof(Interests)); } }
        }

        public List<string> AllInterests => _interestUtil.GetAll();

        public string ErrorMessage
        {
            get => _errorMessage;
            private set { if (_errorMessage != value) { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); } }
        }


        public bool HasLoverType => LoverType != null;


        public string LoverTypeResultText => LoverType switch
        {
            Domain.LoverType.SOCIAL_EXPLORER => "Social Explorer — Extroversion",
            Domain.LoverType.DEEP_THINKER => "Deep Thinker — Introspection",
            Domain.LoverType.ADVENTURE_SEEKER => "Adventure Seeker — Spontaneity",
            Domain.LoverType.STABILITY_LOVER => "Stability Lover — Structure",
            Domain.LoverType.EMPATHETIC_CONNECTOR => "Empathetic Connector — Sensibility",
            _ => "Not determined yet"
        };

        public List<string> ShuffledQuestions
        {
            get => _shuffledQuestions.ToList();
            private set { if (!_shuffledQuestions.SequenceEqual(value ?? new())) { _shuffledQuestions = new(value ?? new()); OnPropertyChanged(nameof(ShuffledQuestions)); } }
        }

        public List<int> Answers
        {
            get => _answers.ToList();
            private set { if (!_answers.SequenceEqual(value ?? new())) { _answers = new(value ?? new()); OnPropertyChanged(nameof(Answers)); } }
        }


        public EditProfileViewModel(int userId, ProfileService profileService, PhotoService photoService,
            QuestionaireUtil questionaireUtil, InterestUtil interestUtil)
        {
            _userId = userId;
            _profileService = profileService;
            _photoService = photoService;
            _questionaireUtil = questionaireUtil;
            _interestUtil = interestUtil;

            LoadProfile();
        }


        private void PopulateFromProfile(DatingProfile profile)
        {
            Name = profile.Name;
            Age = profile.Age;
            StarSign = profile.GetStarSign();
            LoverType = profile.LoverType;
            Bio = profile.Bio;
            Location = profile.Location;
            Nationality = profile.Nationality;
            MaxDistance = profile.MaxDistance;
            MinPreferredAge = profile.MinPreferredAge;
            MaxPreferredAge = profile.MaxPreferredAge;
            Gender = profile.Gender;
            DisplayStarSign = profile.DisplayStarSign;
            IsArchived = profile.IsArchived;
            PreferredGenders = profile.PreferredGenders ?? new List<Gender>();
            Photos = new List<Photo>(profile.Photos ?? new List<Photo>());
            Interests = new List<string>(profile.Interests ?? new List<string>());
        }

        private ProfileData BuildProfileData() => new ProfileData(
            Gender, PreferredGenders, Location, Nationality,
            MaxDistance, MinPreferredAge, MaxPreferredAge,
            Bio, DisplayStarSign,
            new List<Photo>(Photos),
            new List<string>(Interests),
            LoverType
        );


        public void LoadProfile()
        {
            DatingProfile profile = _profileService.GetProfileById(_userId);
            PopulateFromProfile(profile);
            ErrorMessage = string.Empty;
        }

        public void SaveChanges()
        {
            try
            {
                _profileService.UpdateProfile(_userId, BuildProfileData());
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public void DiscardChanges()
        {
            LoadProfile();
        }

        public void ArchiveProfile()
        {
            DatingProfile profile = _profileService.GetProfileById(_userId);
            _profileService.ArchiveProfile(profile);
            IsArchived = true;
        }

        public void UnarchiveProfile()
        {
            DatingProfile profile = _profileService.GetProfileById(_userId);
            _profileService.UnarchiveProfile(profile);
            IsArchived = false;
        }

        public void DeleteProfile()
        {
            DatingProfile profile = _profileService.GetProfileById(_userId);
            _profileService.DeleteProfile(profile);
        }

        public void AddPhoto(Photo photo)
        {
            try
            {
                _photoService.AddPhoto(photo);
                _photos.Add(photo);
                OnPropertyChanged(nameof(Photos));
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public void RemovePhoto(int photoId)
        {
            try
            {
                if (_photos.Count <= 2)
                    throw new Exception("You must have at least 2 photos!");

                _photoService.DeleteById(photoId);
                Photo? toRemove = _photos.FirstOrDefault(p => p.PhotoId == photoId);
                if (toRemove != null) _photos.Remove(toRemove);
                OnPropertyChanged(nameof(Photos));
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public bool CanRemovePhoto() => _photos.Count > 2;

        public void ReorderPhotos(List<int> newPhotoIdOrder)
        {
            try
            {
                _photoService.ReorderPhotos(_userId, newPhotoIdOrder);
                var reorderedPhotos = newPhotoIdOrder
                    .Select(id => _photos.FirstOrDefault(p => p.PhotoId == id))
                    .Where(p => p != null)
                    .ToList()!;
                _photos.Clear();
                foreach (var photo in reorderedPhotos)
                {
                    _photos.Add(photo);
                }
                OnPropertyChanged(nameof(Photos));
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public void AddInterest(string interest)
        {
            try
            {
                if (_interests.Count >= 15)
                    throw new Exception("You cannot have more than 15 interests!");

                if (_interests.Contains(interest))
                    throw new Exception("This interest is already added!");

                _interests.Add(interest);
                OnPropertyChanged(nameof(Interests));
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public void RemoveInterest(string interest)
        {
            try
            {
                if (_interests.Count <= 3)
                    throw new Exception("You must have at least 3 interests!");

                _interests.Remove(interest);
                OnPropertyChanged(nameof(Interests));
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public bool CanAddInterest() => _interests.Count < 15;

        public bool CanRemoveInterest() => _interests.Count > 3;

        public bool CanRemoveInterest(string interest) => _interests.Contains(interest) && _interests.Count > 3;

        public void PrepareQuestionnaire()
        {
            ShuffledQuestions = _questionaireUtil.GetQuestions();
            Answers = Enumerable.Repeat(0, ShuffledQuestions.Count).ToList();
        }

        public void SetAnswer(int questionIndex, int value)
        {
            if (questionIndex < 0 || questionIndex >= _answers.Count) return;
            if (value < 1 || value > 5) return;
            _answers[questionIndex] = value;
            OnPropertyChanged(nameof(Answers));
        }

        public int GetAnswer(int questionIndex)
        {
            if (questionIndex < 0 || questionIndex >= _answers.Count) return 0;
            return _answers[questionIndex];
        }

        public bool CanSubmitQuestionnaire()
            => _answers.Count > 0 && _answers.All(a => a > 0);

        public void CancelQuestionnaire()
        {
            ShuffledQuestions = new List<string>();
            Answers = new List<int>();
        }

        public void SubmitQuestionnaire()
        {
            if (!CanSubmitQuestionnaire())
            {
                ErrorMessage = "Please answer all questions before submitting.";
                return;
            }

            LoverType = _questionaireUtil.CalculateLoveType(_answers.ToList());
            _profileService.UpdateProfile(_userId, BuildProfileData());

            ShuffledQuestions = new List<string>();
            Answers = new List<int>();
            ErrorMessage = string.Empty;

            OnPropertyChanged(nameof(LoverType));
            OnPropertyChanged(nameof(LoverTypeResultText));
            OnPropertyChanged(nameof(HasLoverType));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}