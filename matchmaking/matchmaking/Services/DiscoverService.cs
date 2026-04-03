using matchmaking.Domain;
using matchmaking.Repositories;
using matchmaking.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matchmaking.Services
{
    internal class DiscoverService
    {
        private ProfileRepository ProfileRepo;
        private InteractionRepository InteractionRepo;
        private MockCommunityUtil CommunityUtil;
        private CompatibilityUtil CompatibilityUtil;

        public DiscoverService(ProfileRepository profileRepo, InteractionRepository interactionRepo, MockCommunityUtil communityUtil, CompatibilityUtil compatibilityUtil) { 
            ProfileRepo = profileRepo;
            InteractionRepo = interactionRepo;
            CommunityUtil = communityUtil;
            CompatibilityUtil = compatibilityUtil;
        }

        
        public bool IsProfileArchived(int profileId)
        {
            DatingProfile? profile = ProfileRepo.FindById(profileId);
            return profile != null && profile.IsArchived;
        }

        public List<DatingProfile> GetCandidates(int profileId)
        {
            DatingProfile? user = ProfileRepo.FindById(profileId);
            if (user == null)
            {
                return new List<DatingProfile>();
            }

            List<DatingProfile> allProfiles = ProfileRepo.GetAll();
            List<Interaction> interactions = InteractionRepo.GetAll();

            Debug.WriteLine("All profiles:");
            foreach(var profile in allProfiles)
            {
                Debug.WriteLine($"profile: {profile.UserId}, name: {profile.Name}");
            }

            HashSet<int> seenProfiles = interactions
                .Where(i => i.FromProfileId == profileId)
                .Select(i => i.ToProfileId)
                .ToHashSet();

            List<DatingProfile> candidates = allProfiles
                .Where(p =>
                    p.UserId != profileId && !p.IsArchived && !seenProfiles.Contains(p.UserId) 
                    && user.PreferredGenders.Contains(p.Gender) && p.PreferredGenders.Contains(user.Gender)
                )
                .ToList();

            foreach(var profile in candidates)
            {
                Debug.WriteLine($"candidate: {profile.UserId}, name: {profile.Name}");
            }

            //TODO: Check if it still remains as the first show profile!!!!
            List<DatingProfile> result = new List<DatingProfile>();
            DatingProfile? hotSeatProfile = candidates.FirstOrDefault(p => p.IsHotSeat);

            Debug.WriteLine($"hot seat profile: {(hotSeatProfile != null ? hotSeatProfile.UserId.ToString() : "null")}, name: {(hotSeatProfile != null ? hotSeatProfile.Name : "null")}");
            if (hotSeatProfile != null)
            {
                result.Add(hotSeatProfile);
                candidates.Remove(hotSeatProfile);
            }

            List<DatingProfile> sorted = candidates
            .Select(p => new { Profile = p, Score = CompatibilityUtil.CalculateCompatibility(user, p) })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Select(x => x.Profile)
            .ToList();

            result.AddRange(sorted);

            Debug.WriteLine("final list:");
            foreach(var profile in result)
            {
                Debug.WriteLine($"final candidate: {profile.UserId}, name: {profile.Name}");
            }
            return result;
        }

        public List<string> GetSharedCommunities(int userId1, int userId2)
        {
            return CommunityUtil.GetSharedCommunities(userId1, userId2);
        }
    }
}