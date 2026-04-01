using matchmaking.Domain;
using matchmaking.Services;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace matchmaking.ViewModels
{
    internal class AdminViewModel : ObservableObject
    {
        private readonly SupportTicketService _supportTicketService;
        private readonly ProfileService _profileService;

        private List<SupportTicket> _unresolvedTickets = new();
        private SupportTicket _selectedTicket;
        private List<DatingProfile> _searchResults = new();
        private string _searchQuery = string.Empty;
        private string _errorMessage = string.Empty;

        private readonly RelayCommand _searchCommand;
        private readonly RelayCommand _resolveFoundCommand;
        private readonly RelayCommand _resolveNotFoundCommand;

        public ICommand SearchCommand => _searchCommand;
        public ICommand ResolveFoundCommand => _resolveFoundCommand;
        public ICommand ResolveNotFoundCommand => _resolveNotFoundCommand;

        public List<SupportTicket> UnresolvedTickets
        {
            get => _unresolvedTickets;
            private set => SetProperty(ref _unresolvedTickets, value);
        }

        public SupportTicket SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                if (SetProperty(ref _selectedTicket, value))
                {
                    OnPropertyChanged(nameof(HasSelectedTicket));
                    _resolveFoundCommand.NotifyCanExecuteChanged();
                    _resolveNotFoundCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public bool HasSelectedTicket => SelectedTicket != null;

        public List<DatingProfile> SearchResults
        {
            get => _searchResults;
            private set => SetProperty(ref _searchResults, value);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set => SetProperty(ref _searchQuery, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public AdminViewModel(SupportTicketService supportTicketService, ProfileService profileService)
        {
            _supportTicketService = supportTicketService;
            _profileService = profileService;

            _searchCommand = new RelayCommand(Search);
            _resolveFoundCommand = new RelayCommand(ResolveFound, CanResolve);
            _resolveNotFoundCommand = new RelayCommand(ResolveNotFound, CanResolve);

            LoadUnresolvedTickets();
        }

        public void LoadUnresolvedTickets()
        {
            try
            {
                UnresolvedTickets = _supportTicketService.GetAllUnresolved();
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private void Search()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchQuery))
                {
                    SearchResults = new List<DatingProfile>();
                    return;
                }

                SearchResults = _profileService.SearchByName(SearchQuery);
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private void ResolveFound()
        {
            if (SelectedTicket == null) return;
            try
            {
                _supportTicketService.ResolveTicket(SelectedTicket.Email, true);
                SelectedTicket = null;
                LoadUnresolvedTickets();
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private void ResolveNotFound()
        {
            if (SelectedTicket == null) return;
            try
            {
                _supportTicketService.ResolveTicket(SelectedTicket.Email, false);
                SelectedTicket = null;
                LoadUnresolvedTickets();
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private bool CanResolve() => SelectedTicket != null;
    }
}