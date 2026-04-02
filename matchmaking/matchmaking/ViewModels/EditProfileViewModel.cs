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

        private DatingProfile _profile = null!;
        private ObservableCollection<string> _shuffledQuestions = new();
        private ObservableCollection<int> _answers = new();

        public string Name
        {
            get => _profile.Name;
            set
            {
                if (_profile.Name != value)
                {
                    _profile.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Age => _profile.Age;

        public StarSign StarSign => _profile.GetStarSign();

        public LoverType? LoverType
        {
            get => _profile.LoverType;
            set
            {
                if (_profile.LoverType != value)
                {
                    _profile.LoverType = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasLoverType));
                    OnPropertyChanged(nameof(LoverTypeResultText));
                }
            }
        }

        public string Bio
        {
            get => _profile.Bio;
            set
            {
                if (_profile.Bio != value)
                {
                    _profile.Bio = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Location
        {
            get => _profile.Location;
            set
            {
                if (_profile.Location != value)
                {
                    _profile.Location = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Nationality
        {
            get => _profile.Nationality;
            set
            {
                if (_profile.Nationality != value)
                {
                    _profile.Nationality = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaxDistance
        {
            get => _profile.MaxDistance;
            set
            {
                if (_profile.MaxDistance != value)
                {
                    _profile.MaxDistance = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MinPreferredAge
        {
            get => _profile.MinPreferredAge;
            set
            {
                if (_profile.MinPreferredAge != value)
                {
                    _profile.MinPreferredAge = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaxPreferredAge
        {
            get => _profile.MaxPreferredAge;
            set
            {
                if (_profile.MaxPreferredAge != value)
                {
                    _profile.MaxPreferredAge = value;
                    OnPropertyChanged();
                }
            }
        }

        public Gender Gender
        {
            get => _profile.Gender;
            set
            {
                if (_profile.Gender != value)
                {
                    _profile.Gender = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SelectedGender));
                }
            }
        }

        public List<string> GenderOptions { get; } = new List<string> { "Male", "Female", "Non-Binary", "Other" };

        public string SelectedGender
        {
            get => _profile.Gender switch
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
                if (_profile.Gender != newGender)
                {
                    _profile.Gender = newGender;
                    OnPropertyChanged(nameof(Gender));
                    OnPropertyChanged(nameof(SelectedGender));
                }
            }
        }

        public bool DisplayStarSign
        {
            get => _profile.DisplayStarSign;
            set
            {
                if (_profile.DisplayStarSign != value)
                {
                    _profile.DisplayStarSign = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsArchived
        {
            get => _profile.IsArchived;
            set
            {
                if (_profile.IsArchived != value)
                {
                    _profile.IsArchived = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsNotArchived));
                    ArchiveProfileCommand.NotifyCanExecuteChanged();
                    UnarchiveProfileCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public bool IsNotArchived => !IsArchived;

        public List<Gender> PreferredGenders
        {
            get => _profile.PreferredGenders;
            set
            {
                _profile.PreferredGenders = value ?? new List<Gender>();
                OnPropertyChanged();
            }
        }

        public List<Photo> Photos
        {
            get => _profile.Photos;
            private set
            {
                _profile.Photos = value ?? new List<Photo>();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentPhotoCount));
            }
        }

        public List<string> Interests
        {
            get => _profile.Interests;
            private set
            {
                _profile.Interests = value ?? new List<string>();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentInterestCount));
            }
        }

        public List<string> AllInterests => _interestUtil.GetAll();

        public bool HasLoverType => LoverType != null;

        public int CurrentInterestCount => _profile.Interests.Count;

        public int CurrentPhotoCount => _profile.Photos.Count;

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

        private void NotifyAllProperties()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Age));
            OnPropertyChanged(nameof(StarSign));
            OnPropertyChanged(nameof(LoverType));
            OnPropertyChanged(nameof(HasLoverType));
            OnPropertyChanged(nameof(LoverTypeResultText));
            OnPropertyChanged(nameof(Bio));
            OnPropertyChanged(nameof(Location));
            OnPropertyChanged(nameof(Nationality));
            OnPropertyChanged(nameof(MaxDistance));
            OnPropertyChanged(nameof(MinPreferredAge));
            OnPropertyChanged(nameof(MaxPreferredAge));
            OnPropertyChanged(nameof(Gender));
            OnPropertyChanged(nameof(SelectedGender));
            OnPropertyChanged(nameof(DisplayStarSign));
            OnPropertyChanged(nameof(IsArchived));
            OnPropertyChanged(nameof(IsNotArchived));
            OnPropertyChanged(nameof(PreferredGenders));
            OnPropertyChanged(nameof(Photos));
            OnPropertyChanged(nameof(CurrentPhotoCount));
            OnPropertyChanged(nameof(Interests));
            OnPropertyChanged(nameof(CurrentInterestCount));
            ArchiveProfileCommand.NotifyCanExecuteChanged();
            UnarchiveProfileCommand.NotifyCanExecuteChanged();
        }

        private ProfileData BuildProfileData() => new ProfileData(
            _profile.Name, _profile.Gender, _profile.PreferredGenders,
            _profile.Location, _profile.Nationality, _profile.MaxDistance,
            _profile.MinPreferredAge, _profile.MaxPreferredAge,
            _profile.Bio, _profile.DisplayStarSign,
            new List<Photo>(_profile.Photos),
            new List<string>(_profile.Interests),
            _profile.LoverType
        );

        public void LoadProfile()
        {
            _profile = _profileService.GetProfileById(_userId);
            NotifyAllProperties();
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
            _profileService.ArchiveProfile(_profile);
            IsArchived = true;
        }

        public void UnarchiveProfile()
        {
            _profileService.UnarchiveProfile(_profile);
            IsArchived = false;
        }

        public void DeleteProfile()
        {
            _profileService.DeleteProfile(_profile);
        }

        public void AddPhoto(Photo photo)
        {
            _photoService.AddPhoto(photo);
            _profile.Photos.Add(photo);
            OnPropertyChanged(nameof(Photos));
            OnPropertyChanged(nameof(CurrentPhotoCount));
        }

        public void RemovePhoto(int photoId)
        {
            if (_profile.Photos.Count <= 2)
                return;

            _photoService.DeleteById(photoId);
            Photo? toRemove = _profile.Photos.FirstOrDefault(p => p.PhotoId == photoId);
            if (toRemove != null) _profile.Photos.Remove(toRemove);
            OnPropertyChanged(nameof(Photos));
            OnPropertyChanged(nameof(CurrentPhotoCount));
        }

        public bool CanRemovePhoto() => _profile.Photos.Count > 2;

        public void ReorderPhotos(List<int> newPhotoIdOrder)
        {
            _photoService.ReorderPhotos(_userId, newPhotoIdOrder);
            var reorderedPhotos = newPhotoIdOrder
                .Select(id => _profile.Photos.FirstOrDefault(p => p.PhotoId == id))
                .Where(p => p != null)
                .ToList()!;
            _profile.Photos.Clear();
            foreach (var photo in reorderedPhotos)
            {
                _profile.Photos.Add(photo);
            }
            OnPropertyChanged(nameof(Photos));
            OnPropertyChanged(nameof(CurrentPhotoCount));
        }

        public void AddInterest(string interest)
        {
            if (_profile.Interests.Count >= 15 || _profile.Interests.Contains(interest))
                return;

            _profile.Interests.Add(interest);
            OnPropertyChanged(nameof(Interests));
            OnPropertyChanged(nameof(CurrentInterestCount));
        }

        public void RemoveInterest(string interest)
        {
            if (_profile.Interests.Count <= 3)
                return;

            _profile.Interests.Remove(interest);
            OnPropertyChanged(nameof(Interests));
            OnPropertyChanged(nameof(CurrentInterestCount));
        }

        public bool CanAddInterest() => _profile.Interests.Count < 15;

        public bool CanRemoveInterest() => _profile.Interests.Count > 3;

        public bool CanRemoveInterest(string interest) => _profile.Interests.Contains(interest) && _profile.Interests.Count > 3;

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
