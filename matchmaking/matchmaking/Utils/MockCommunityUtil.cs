using System;
using System.Collections.Generic;
using System.Text;

namespace matchmaking.Utils
{
    public class MockCommunityUtil
    {
        private Dictionary<int, List<string>> userCommunities = new Dictionary<int, List<string>>
        {
            { 1, new List<string> { "Hiking", "Coffee Lovers", "Photography" } },
            { 2, new List<string> { "Books", "Cat Lovers", "Yoga" } },
            { 3, new List<string> { "Gym", "Cooking", "Hiking" } },
            { 4, new List<string> { "Photography", "Travel", "Coffee Lovers" } },
            { 5, new List<string> { "Music Production", "Gaming", "Night Life" } },
            { 6, new List<string> { "Yoga", "Meditation", "Books" } },
            { 7, new List<string> { "Gaming", "Software Dev", "Music Production" } },
            { 8, new List<string> { "Art", "Dog Lovers", "Photography" } },
            { 9, new List<string> { "Football", "Gym", "Cooking" } },
            { 10, new List<string> { "Cooking", "Food Blogging", "Travel" } }
        };

        public List<string>? GetUserCommunities(int userId)
        {
            if (userCommunities.ContainsKey(userId))
            {
                return userCommunities[userId];
            }
            return null;
        }

        public List<string> GetSharedCommunities(int userId1, int userId2)
        {
            List<string> commonCommunities = new List<string>();
            if (userCommunities.ContainsKey(userId1) && userCommunities.ContainsKey(userId2))
            {
                foreach (string community in userCommunities[userId2])
                {
                    if (userCommunities[userId1].Contains(community)){
                        commonCommunities.Add(community);
                    }
                }
            }
            return commonCommunities;
        }
    }
}
