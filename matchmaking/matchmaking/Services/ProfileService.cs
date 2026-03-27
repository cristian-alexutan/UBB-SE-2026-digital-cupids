using matchmaking.Domain;
using matchmaking.Repositories;
using matchmaking.Utils;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.System;

namespace matchmaking.Services
{
    internal class ProfileService
    {
        private ProfileRepository ProfileRepo;
        private MockUserUtil UserUtil;

        public ProfileService(ProfileRepository repo,MockUserUtil userUtil) {
            ProfileRepo = repo;
            UserUtil = userUtil;
        }
        private int CalculateAge(DateTime dateOfBirth)
        {
            int age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now < dateOfBirth.AddYears(age))
                age--;
            return age;
        }



        public DatingProfile CreateProfile(int Id,ProfileData profileData)
        {
            UserData userData = UserUtil.GetUserData(Id);
            int age = CalculateAge(userData.Birthdate);
            DatingProfile newProfile = new DatingProfile(
                profileData.Gender,
                profileData.PreferredGenders,
                profileData.Location,
                profileData.Nationality,
                profileData.MaxDistance,
                age,
                profileData.MinPreferredAge,
                profileData.MaxPreferredAge,
                profileData.Bio,
                profileData.DisplayStarSign,
                false,
                profileData.Photos,
                profileData.Interests,
                userData.Birthdate,
                profileData.LoverType,
                false,
                false,
                0,
                0
            );

            ProfileRepo.Add(newProfile);
            return newProfile;
        }
        public DatingProfile UpdateProfile(int id, ProfileData profileData)
        {
            DatingProfile existingProfile = ProfileRepo.FindById(id);

            existingProfile.Gender = profileData.Gender;
            existingProfile.PreferredGenders = profileData.PreferredGenders;
            existingProfile.Location = profileData.Location;
            existingProfile.MaxDistance = profileData.MaxDistance;
            existingProfile.MinPreferredAge = profileData.MinPreferredAge;
            existingProfile.MaxPreferredAge = profileData.MaxPreferredAge;
            existingProfile.Bio = profileData.Bio;
            existingProfile.DisplayStarSign = profileData.DisplayStarSign;
            existingProfile.Photos = profileData.Photos;
            existingProfile.Interests = profileData.Interests;
            existingProfile.LoverType = profileData.LoverType;

            return ProfileRepo.Update(existingProfile);
        }
        public DatingProfile DeleteProfile(DatingProfile profile)
        {
            return ProfileRepo.DeleteById(profile.UserId);
        }
        public void ArchiveProfile(DatingProfile profile)
        {
            profile.IsArchived = true;
            ProfileRepo.Update(profile);
        }

        public void UnarchiveProfile(DatingProfile profile)
        {
            profile.IsArchived = false;
            ProfileRepo.Update(profile);
        }
        public DatingProfile GetProfileById(int id)
        {
            return ProfileRepo.FindById(id);
        }

        public int GetUserId(DatingProfile profile)
        {
            return profile.UserId;
        }
        public void SetHotSeat(int userId)
        {
            DatingProfile profile = ProfileRepo.FindById(userId);
            profile.IsHotSeat = true;
            profile.HotSeatDay = DateTime.Now.DayOfYear;
            ProfileRepo.Update(profile);
        }

        public void ResetHotSeat()
        {
            List<DatingProfile> profiles = ProfileRepo.GetAll();
            foreach (DatingProfile profile in profiles)
            {
                if (profile.IsHotSeat)
                {
                    profile.IsHotSeat = false;
                    ProfileRepo.Update(profile);
                }
            }
        }
        public List<DatingProfile> SearchByName(string name)
        {
            List<DatingProfile> allProfiles = ProfileRepo.GetAll();
            return allProfiles
                .Where(p => UserUtil.GetUserData(p.UserId).Username
                    .Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

    }
}
