using matchmaking.Domain;
using matchmaking.Services;
using matchmaking.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace matchmaking.ViewModels
{
    internal class SplashViewModel : INotifyPropertyChanged
    {

        private readonly int _userId;
        private readonly MockUserUtil _mockUserUtil;
        private readonly ProfileService _profileService;
        private readonly DatingAdminService _datingAdminService;

        public event PropertyChangedEventHandler? PropertyChanged;


        public SplashViewModel(int userId, MockUserUtil mockUserUtil, ProfileService profileService, DatingAdminService datingAdminService)
        {
            _userId = userId;
            _mockUserUtil = mockUserUtil;
            _profileService = profileService;
            _datingAdminService = datingAdminService;
        }

        public bool IsUserAdult()
        {
            UserData userData = _mockUserUtil.GetUserData(_userId);

            DateTime today = DateTime.Today;
            int age = today.Year - userData.Birthdate.Year;

            if (userData.Birthdate.Date > today.AddYears(-age))
            {
                age--;
            }

            return age >= 18;
        }
        public bool HasProfile()
        {
            try
            {
                DatingProfile profile = _profileService.GetProfileById(_userId);
                return profile != null;
            }
            catch
            {
                return false;
            }
        }

        public bool IsAdmin()
        {
            return _datingAdminService.IsAdmin(_userId);
        }

        public Screen DecideNextScreen()
        {
            if (!IsUserAdult())
            {
                return Screen.AGE_BLOCK;
            }

            if (IsAdmin())
            {
                return Screen.ADMIN;
            }

            if (!HasProfile())
            {
                return Screen.CREATE;
            }

            return Screen.DISCOVER;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}