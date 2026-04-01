using matchmaking.Domain;
using matchmaking.Services;
using matchmaking.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace matchmaking.ViewModels
{
    internal class EditProfileViewModel : ObservableObject
    {
        public int _userId { get; }
        private readonly ProfileService _profileService;
        private readonly PhotoService _photoService;
        private readonly QuestionaireUtil _questionaireUtil;
        private readonly InterestUtil _interestUtil;

        private string _name = string.Empty;
        private int _age;
        private StarSign _starSign;
        private LoverType? _loverType;
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
        private ObservableCollection<string> _shuffledQuestions = new();
        private ObservableCollection<int> _answers = new();

        public string Name
        {
            get => _name;
            private set => SetProperty(ref _name, value);
        }

        public int Age
        {
            get => _age;
            private set => SetProperty(ref _age, value);
        }

        public StarSign StarSign
        {
            get => _starSign;
            private set => SetProperty(ref _starSign, value);
        }

        public LoverType? LoverType
        {
            get => _loverType;
            private set
            {
                if (SetProperty(ref _loverType, value))
                {
                    OnPropertyChanged(nameof(HasLoverType));
                    OnPropertyChanged(nameof(LoverTypeResultText));
                }
            }
        }

        public string Bio
        {
            get => _bio;
            set => SetProperty(ref _bio, value);
        }

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public string Nationality
        {
            get => _nationality;
            set => SetProperty(ref _nationality, value);
        }

        public int MaxDistance
        {
            get => _maxDistance;
            set => SetProperty(ref _maxDistance, value);
        }

        public int MinPreferredAge
        {
            get => _minPreferredAge;
            set => SetProperty(ref _minPreferredAge, value);
        }

        public int MaxPreferredAge
        {
            get => _maxPreferredAge;
            set => SetProperty(ref _maxPreferredAge, value);
        }

        public Gender Gender
        {
            get => _gender;
            set
            {
                if (SetProperty(ref _gender, value))
                {
                    OnPropertyChanged(nameof(SelectedGender));
                }
            }
        }

        public List<string> GenderOptions { get; } = new List<string> { "Male", "Female", "Non-Binary", "Other" };

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
            set => SetProperty(ref _displayStarSign, value);
        }

        public bool IsArchived
        {
            get => _isArchived;
            set
            {
                if (SetProperty(ref _isArchived, value))
                {
                    OnPropertyChanged(nameof(IsNotArchived));
                    ArchiveProfileCommand.NotifyCanExecuteChanged();
                    UnarchiveProfileCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public bool IsNotArchived => !IsArchived;

        public List<Gender> PreferredGenders
        {
            get => _preferredGenders;
            set => SetProperty(ref _preferredGenders, value);
        }

        public List<Photo> Photos
        {
            get => _photos.ToList();
            private set
            {
                if (!_photos.SequenceEqual(value ?? new()))
                {
                    _photos = new(value ?? new());
                    OnPropertyChanged(nameof(Photos));
                    OnPropertyChanged(nameof(CurrentPhotoCount));
                }
            }
        }

        public List<string> Interests
        {
            get => _interests.ToList();
            private set
            {
                if (!_interests.SequenceEqual(value ?? new()))
                {
                    _interests = new(value ?? new());
                    OnPropertyChanged(nameof(Interests));
                    OnPropertyChanged(nameof(CurrentInterestCount));
                }
            }
        }

        public List<string> AllInterests => _interestUtil.GetAll();

        public bool HasLoverType => LoverType != null;

        public int CurrentInterestCount => _interests.Count;

        public int CurrentPhotoCount => _photos.Count;

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
            private set
            {
                if (!_shuffledQuestions.SequenceEqual(value ?? new()))
                {
                    _shuffledQuestions = new(value ?? new());
                    OnPropertyChanged(nameof(ShuffledQuestions));
                }
            }
        }

        public List<int> Answers
        {
            get => _answers.ToList();
            private set
            {
                if (!_answers.SequenceEqual(value ?? new()))
                {
                    _answers = new(value ?? new());
                    OnPropertyChanged(nameof(Answers));
                }
            }
        }

        public RelayCommand SaveChangesCommand { get; }
        public RelayCommand DiscardChangesCommand { get; }
        public RelayCommand ArchiveProfileCommand { get; }
        public RelayCommand UnarchiveProfileCommand { get; }
        public RelayCommand DeleteProfileCommand { get; }
        public RelayCommand PrepareQuestionnaireCommand { get; }
        public RelayCommand SubmitQuestionnaireCommand { get; }
        public RelayCommand CancelQuestionnaireCommand { get; }

        public EditProfileViewModel(int userId, ProfileService profileService, PhotoService photoService,
            QuestionaireUtil questionaireUtil, InterestUtil interestUtil)
        {
            _userId = userId;
            _profileService = profileService;
            _photoService = photoService;
            _questionaireUtil = questionaireUtil;
            _interestUtil = interestUtil;

            SaveChangesCommand = new RelayCommand(SaveChanges);
            DiscardChangesCommand = new RelayCommand(DiscardChanges);
            ArchiveProfileCommand = new RelayCommand(ArchiveProfile, () => !IsArchived);
            UnarchiveProfileCommand = new RelayCommand(UnarchiveProfile, () => IsArchived);
            DeleteProfileCommand = new RelayCommand(DeleteProfile);
            PrepareQuestionnaireCommand = new RelayCommand(PrepareQuestionnaire);
            SubmitQuestionnaireCommand = new RelayCommand(SubmitQuestionnaire, CanSubmitQuestionnaire);
            CancelQuestionnaireCommand = new RelayCommand(CancelQuestionnaire);

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
        }

        public void SaveChanges()
        {
            _profileService.UpdateProfile(_userId, BuildProfileData());
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
            _photoService.AddPhoto(photo);
            _photos.Add(photo);
            OnPropertyChanged(nameof(Photos));
            OnPropertyChanged(nameof(CurrentPhotoCount));
        }

        public void RemovePhoto(int photoId)
        {
            if (_photos.Count <= 2)
                return;

            _photoService.DeleteById(photoId);
            Photo? toRemove = _photos.FirstOrDefault(p => p.PhotoId == photoId);
            if (toRemove != null) _photos.Remove(toRemove);
            OnPropertyChanged(nameof(Photos));
            OnPropertyChanged(nameof(CurrentPhotoCount));
        }

        public bool CanRemovePhoto() => _photos.Count > 2;

        public void ReorderPhotos(List<int> newPhotoIdOrder)
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
            OnPropertyChanged(nameof(CurrentPhotoCount));
        }

        public void AddInterest(string interest)
        {
            if (_interests.Count >= 15 || _interests.Contains(interest))
                return;

            _interests.Add(interest);
            OnPropertyChanged(nameof(Interests));
            OnPropertyChanged(nameof(CurrentInterestCount));
        }

        public void RemoveInterest(string interest)
        {
            if (_interests.Count <= 3)
                return;

            _interests.Remove(interest);
            OnPropertyChanged(nameof(Interests));
            OnPropertyChanged(nameof(CurrentInterestCount));
        }

        public bool CanAddInterest() => _interests.Count < 15;

        public bool CanRemoveInterest() => _interests.Count > 3;

        public bool CanRemoveInterest(string interest) => _interests.Contains(interest) && _interests.Count > 3;

        public void PrepareQuestionnaire()
        {
            ShuffledQuestions = _questionaireUtil.GetQuestions();
            Answers = Enumerable.Repeat(0, ShuffledQuestions.Count).ToList();
            SubmitQuestionnaireCommand.NotifyCanExecuteChanged();
        }

        public void SetAnswer(int questionIndex, int value)
        {
            if (questionIndex < 0 || questionIndex >= _answers.Count) return;
            if (value < 1 || value > 5) return;
            _answers[questionIndex] = value;
            OnPropertyChanged(nameof(Answers));
            SubmitQuestionnaireCommand.NotifyCanExecuteChanged();
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
            SubmitQuestionnaireCommand.NotifyCanExecuteChanged();
        }

        public void SubmitQuestionnaire()
        {
            if (!CanSubmitQuestionnaire())
                return;

            LoverType = _questionaireUtil.CalculateLoveType(_answers.ToList());
            _profileService.UpdateProfile(_userId, BuildProfileData());

            ShuffledQuestions = new List<string>();
            Answers = new List<int>();

            SubmitQuestionnaireCommand.NotifyCanExecuteChanged();
        }
    }
}