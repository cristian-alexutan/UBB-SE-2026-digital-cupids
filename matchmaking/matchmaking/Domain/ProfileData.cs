using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matchmaking.Domain
{
    internal class ProfileData
    {
        public Gender Gender { get; set; }
        public List<Gender> PreferredGenders { get; set; }
        public string Location { get; set; }
        public string Nationality { get; set; }

        public int MaxDistance { get; set; }
        public int MinPreferredAge { get; set; }
        public int MaxPreferredAge { get; set; }
        public string Bio { get; set; }
        public bool DisplayStarSign { get; set; }
        public List<Photo> Photos { get; set; }
        public List<String> Interests { get; set; }
        public LoverType? LoverType { get; set; }

        public ProfileData(Gender gender, List<Gender> preferredGenders, string location,string nationality, int maxDistance, int minPreferredAge, int maxPreferredAge, string bio, bool displayStarSign, List<Photo> photos, List<string> interests, LoverType? loverType)
        {
            Gender = gender;
            PreferredGenders = preferredGenders;
            Location = location;
            Nationality = nationality;
            MaxDistance = maxDistance;
            MinPreferredAge = minPreferredAge;
            MaxPreferredAge = maxPreferredAge;
            Bio = bio;
            DisplayStarSign = displayStarSign;
            Photos = photos;
            Interests = interests;
            LoverType = loverType;

        }
    }
}
