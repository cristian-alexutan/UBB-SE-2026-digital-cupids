using matchmaking.Domain;
using matchmaking.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace matchmaking.ViewModels
{
    internal class SpouseCheckerViewModel : INotifyPropertyChanged
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
            set { if (_email != value) { _email = value; OnPropertyChanged(); } }
        }

        public string PartnerName
        {
            get => _partnerName;
            set { if (_partnerName != value) { _partnerName = value; OnPropertyChanged(); } }
        }

        public string MarriageCertificatePath
        {
            get => _marriageCertificatePath;
            set { if (_marriageCertificatePath != value) { _marriageCertificatePath = value; OnPropertyChanged(); } }
        }

        public string PartnerPhotoPath
        {
            get => _partnerPhotoPath;
            set { if (_partnerPhotoPath != value) { _partnerPhotoPath = value; OnPropertyChanged(); } }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set { if (_errorMessage != value) { _errorMessage = value; OnPropertyChanged(); } }
        }

        public SpouseCheckerViewModel(SupportTicketService supportTicketService)
        {
            _supportTicketService = supportTicketService;
        }

        public bool CanSubmit() =>
            !string.IsNullOrWhiteSpace(Email) &&
            !string.IsNullOrWhiteSpace(PartnerName) &&
            !string.IsNullOrWhiteSpace(MarriageCertificatePath) &&
            !string.IsNullOrWhiteSpace(PartnerPhotoPath);

        public void Submit()
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

        public void Cancel() => ClearForm();

        private void ClearForm()
        {
            Email = string.Empty;
            PartnerName = string.Empty;
            MarriageCertificatePath = string.Empty;
            PartnerPhotoPath = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}