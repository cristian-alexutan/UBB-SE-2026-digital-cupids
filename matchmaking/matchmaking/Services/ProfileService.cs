using matchmaking.Domain;
using matchmaking.Repositories;
using matchmaking.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        private void CopyPhotosToStorage(List<Photo> photos)
        {
            string[] allowedExtensions = new string[] { ".jpeg", ".jpg", ".png" };
            const long maxFileSize = 10 * 1024 * 1024;
            string storageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StoredPhotos");
            Directory.CreateDirectory(storageDirectory);

            for (int i = 0; i < photos.Count; i++)
            {
                string extension = Path.GetExtension(photos[i].Location).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    throw new InvalidOperationException("Only JPEG and PNG files are allowed!");
                }

                long fileSize = new FileInfo(photos[i].Location).Length;
                if (fileSize > maxFileSize)
                {
                    throw new InvalidOperationException("The file is too large! Maximum size is 10MB.");
                }

                string fileName = Guid.NewGuid().ToString() + extension;
                string destinationPath = Path.Combine(storageDirectory, fileName);
                File.Copy(photos[i].Location, destinationPath);
                photos[i].Location = destinationPath;
                photos[i].ProfileOrderIndex = i;
            }
        }



        public DatingProfile CreateProfile(int Id,ProfileData profileData)
        {
            UserData userData = UserUtil.GetUserData(Id);
            int age = CalculateAge(userData.Birthdate);

            CopyPhotosToStorage(profileData.Photos);

            DatingProfile newProfile = new DatingProfile(
                Id,
                profileData.Name,
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

            existingProfile.Name = profileData.Name;
            existingProfile.Nationality = profileData.Nationality;
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
                .Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<DatingProfile> GetAllProfiles()
        {
            return ProfileRepo.GetAll();
        }

        public void UpdateBoost(int userId)
        {
            DatingProfile? profile = ProfileRepo.FindById(userId);
            if (profile == null)
            {
                throw new Exception("Profile not found.");
            }
            profile.IsBoosted = true;
            profile.BoostDay = DateTime.Today.Day;
            ProfileRepo.Update(profile);
        }

    }
}
