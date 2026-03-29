using matchmaking.Domain;
using matchmaking.Repositories;
using matchmaking.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matchmaking.Services
{
    public class DiscoverService
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

        
        public List<DatingProfile> GetCandidates(int profileId)
        {
            DatingProfile? user = ProfileRepo.FindById(profileId);
            if (user == null)
            {
                return new List<DatingProfile>();
            }

            List<DatingProfile> allProfiles = ProfileRepo.GetAll();
            List<Interaction> interactions = InteractionRepo.GetAll();

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

            List<DatingProfile> result = new List<DatingProfile>();
            DatingProfile? hotSeatProfile = candidates.FirstOrDefault(p => p.IsHotSeat);
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
            return result;
        }

        public List<string> GetSharedCommunities(int userId1, int userId2)
        {
            return CommunityUtil.GetSharedCommunities(userId1, userId2);
        }

        public DatingProfile? GetProfile(int userId)
        {
            return ProfileRepo.FindById(userId);
        }
    }
}