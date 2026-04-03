using System;
using System.Collections.Generic;
using System.Text;

namespace matchmaking.Utils
{
    internal class MockCommunityUtil
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
            { 10, new List<string> { "Cooking", "Food Blogging", "Travel" } },
            { 11, new List<string> { "Hiking", "Camping", "Photography" } },
            { 12, new List<string> { "Books", "Writing", "Coffee Lovers" } },
            { 13, new List<string> { "Gym", "Running", "Nutrition" } },
            { 14, new List<string> { "Travel", "Photography", "Blogging" } },
            { 15, new List<string> { "Gaming", "Streaming", "Esports" } },
            { 16, new List<string> { "Yoga", "Wellness", "Meditation" } },
            { 17, new List<string> { "Software Dev", "AI", "Gaming" } },
            { 18, new List<string> { "Art", "Design", "Photography" } },
            { 19, new List<string> { "Football", "Fitness", "Sports Analytics" } },
            { 20, new List<string> { "Cooking", "Baking", "Food Blogging" } },
            { 21, new List<string> { "Hiking", "Nature", "Travel" } },
            { 22, new List<string> { "Books", "Philosophy", "Writing" } },
            { 23, new List<string> { "Gym", "CrossFit", "Nutrition" } },
            { 24, new List<string> { "Photography", "Videography", "Travel" } },
            { 25, new List<string> { "Gaming", "VR", "Tech" } },
            { 26, new List<string> { "Yoga", "Pilates", "Wellness" } },
            { 27, new List<string> { "Software Dev", "Open Source", "AI" } },
            { 28, new List<string> { "Art", "Illustration", "Design" } },
            { 29, new List<string> { "Football", "Running", "Fitness" } },
            { 30, new List<string> { "Cooking", "Healthy Eating", "Meal Prep" } }
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
