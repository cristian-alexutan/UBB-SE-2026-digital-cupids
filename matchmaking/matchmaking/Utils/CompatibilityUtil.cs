using matchmaking.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace matchmaking.Utils
{
    internal class CompatibilityUtil
    {

        private LocationUtil locationUtil = new LocationUtil();
        private MockCommunityUtil communityUtil = new MockCommunityUtil();

        public CompatibilityUtil() { }

        public float CalculateCompatibility(DatingProfile profile1, DatingProfile profile2)
        {
            if (profile1 != null && profile2 != null)
            {
                if (!profile1.PreferredGenders.Contains(profile2.Gender) ||
                   !profile2.PreferredGenders.Contains(profile1.Gender))
                {
                    return 0;
                }
            }

            int d1 = profile1.MaxDistance;
            int d2 = profile2.MaxDistance;

            var coords1 = locationUtil.GetCoords(profile1.Location);
            var coords2 = locationUtil.GetCoords(profile2.Location);

            if (coords1 == null || coords2 == null)
            {
                return 0;
            }

            (float lat1, float lon1) = coords1;
            (float lat2, float lon2) = coords2;
            float D = CalculateDistance(lat1, lon1, lat2, lon2);

            int a1 = profile1.Age;
            int a2 = profile2.Age;

            int l1 = profile1.MinPreferredAge;
            int r1 = profile1.MaxPreferredAge;
            int l2 = profile2.MinPreferredAge;
            int r2 = profile2.MaxPreferredAge;

            int NC = communityUtil.GetSharedCommunities(profile1.UserId, profile2.UserId).Count();
            int NI = profile1.Interests.Intersect(profile2.Interests).Count();

            int b1 = profile1.IsBoosted ? 1 : 0;
            int b2 = profile2.IsBoosted ? 1 : 0;

            if (a1 < l2 || a1 > r2 || a2 < l1 || a2 > r1)
                return 0;

            if (D > d1 + 10 || D > d2 + 10)
                return 0;

            float score = b1 * 40 + b2 * 40 + 50 + NC * 7 + NI * 5 - (float)Math.Pow(Math.Floor(D / 10.0), 1.7);
            return score <= 0 ? 0 : score;
        }

        private float CalculateDistance(float lat1, float lon1, float lat2, float lon2)
        {
            const float R = 6371f;
            float dLat = ToRad(lat2 - lat1);
            float dLon = ToRad(lon2 - lon1);

            float a = (float)(Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                      Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                      Math.Sin(dLon / 2) * Math.Sin(dLon / 2));

            float c = 2 * (float)Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private float ToRad(float degrees)
        {
            return degrees * (float)(Math.PI / 180);
        }
    }
}