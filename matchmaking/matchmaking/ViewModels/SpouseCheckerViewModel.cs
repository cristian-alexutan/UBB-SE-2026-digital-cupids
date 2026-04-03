using matchmaking.Domain;
using matchmaking.Services;
using System;
using System.Windows.Input;

namespace matchmaking.ViewModels
{
    internal class SpouseCheckerViewModel : ObservableObject
    {
        private readonly SupportTicketService _supportTicketService;

        private string _email = string.Empty;
        private string _partnerName = string.Empty;
        private string _marriageCertificatePath = string.Empty;
        private string _partnerPhotoPath = string.Empty;
        private string _errorMessage = string.Empty;

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    _submitCommand.NotifyCanExecuteChanged();
                    OnPropertyChanged(nameof(CanSubmitForm));
                }
            }
        }

        public string PartnerName
        {
            get => _partnerName;
            set
            {
                if (SetProperty(ref _partnerName, value))
                {
                    _submitCommand.NotifyCanExecuteChanged();
                    OnPropertyChanged(nameof(CanSubmitForm));
                }
            }
        }

        public string MarriageCertificatePath
        {
            get => _marriageCertificatePath;
            set
            {
                if (SetProperty(ref _marriageCertificatePath, value))
                {
                    _submitCommand.NotifyCanExecuteChanged();
                    OnPropertyChanged(nameof(CanSubmitForm));
                }
            }
        }

        public string PartnerPhotoPath
        {
            get => _partnerPhotoPath;
            set
            {
                if (SetProperty(ref _partnerPhotoPath, value))
                {
                    _submitCommand.NotifyCanExecuteChanged();
                    OnPropertyChanged(nameof(CanSubmitForm));
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public bool CanSubmitForm => CanSubmit();

        private readonly RelayCommand _submitCommand;
        private readonly RelayCommand _cancelCommand;

        public ICommand SubmitCommand => _submitCommand;
        public ICommand CancelCommand => _cancelCommand;

        public SpouseCheckerViewModel(SupportTicketService supportTicketService)
        {
            _supportTicketService = supportTicketService;
            _submitCommand = new RelayCommand(Submit, CanSubmit);
            _cancelCommand = new RelayCommand(Cancel);
        }

        private bool CanSubmit() =>
            !string.IsNullOrWhiteSpace(Email) &&
            !string.IsNullOrWhiteSpace(PartnerName) &&
            !string.IsNullOrWhiteSpace(MarriageCertificatePath) &&
            !string.IsNullOrWhiteSpace(PartnerPhotoPath);

        private void Submit()
        {
            try
            {
                var ticket = new SupportTicket(
                    Email, PartnerName, MarriageCertificatePath, PartnerPhotoPath, isResolved: false);

                _supportTicketService.CreateTicket(ticket);
                ClearForm();
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private void Cancel() => ClearForm();

        private void ClearForm()
        {
            Email = string.Empty;
            PartnerName = string.Empty;
            MarriageCertificatePath = string.Empty;
            PartnerPhotoPath = string.Empty;
            OnPropertyChanged(nameof(CanSubmitForm));
        }
    }
}