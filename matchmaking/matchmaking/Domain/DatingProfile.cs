using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace matchmaking.Domain
{
    internal enum Gender
    { 
        MALE,
        FEMALE,
        NON_BINARY,
        OTHER
    }
    internal enum LoverType
    {
       SOCIAL_EXPLORER,
       DEEP_THINKER,
       ADVENTURE_SEEKER,
       STABILITY_LOVER,
       EMPHATETIC_CONNECTOR
    }
    internal enum StarSign
    {
        ARIES,
        TAURUS,
        GEMINI,
        CANCER,
        LEO,
        VIRGO,
        LIBRA,
        SCORPIO,
        SAGITTARIUS,
        CAPRICORN,
        AQUARIUS,
        PISCES
    }
    internal class DatingProfile
    {
        public int UserId { get; }
        public Gender Gender { get; set; }
        public List<Gender> PreferredGenders { get; set; }
        public string Location { get; set; }
        public string Nationality { get; set; }
        public int MaxDistance { get; set; }
        public int Age { get;}
        public int MinPreferredAge { get; set; }
        public int MaxPreferredAge { get; set; }
        public string Bio { get; set; }
        public bool DisplayStarSign { get; set; }
        public bool IsArchived { get; set; }
        public List<Photo> Photos { get; set; }
        public List<String> Interests { get; set; }
        public DateTime DateOfBirth { get; }
        public LoverType? LoverType { get; set; }
        public bool IsHotSeat { get; set; }
        public bool IsBoosted { get; set; }
        public int BoostDay { get; set; }
        public int HotSeatDay { get; set; }

        public DatingProfile(int userId, Gender gender, List<Gender> preferredGenders, string location, string nationality, int maxDistance, int age, int minPreferredAge, int maxPreferredAge, string bio, bool displayStarSign,bool isArchived, List<Photo> photos, List<string> interests, DateTime dateOfBirth, LoverType? loverType, bool isHotSeat, bool isBoosted, int boostDay, int hotSeatDay)

        {
            UserId = userId;
            Gender= gender;
            PreferredGenders = preferredGenders;
            Location = location;
            Nationality = nationality;
            MaxDistance = maxDistance;
            Age = age;
            MinPreferredAge = minPreferredAge;
            MaxPreferredAge = maxPreferredAge;
            Bio = bio;
            DisplayStarSign = displayStarSign;
            IsArchived = isArchived;
            Photos = photos;
            Interests = interests;
            DateOfBirth = dateOfBirth;
            LoverType = loverType;
            IsHotSeat = isHotSeat;
            IsBoosted = isBoosted;
            BoostDay = boostDay;
            HotSeatDay = hotSeatDay;
        }
        public DatingProfile( Gender gender, List<Gender> preferredGenders, string location, string nationality, int maxDistance, int age, int minPreferredAge, int maxPreferredAge, string bio, bool displayStarSign, bool isArchived, List<Photo> photos, List<string> interests, DateTime dateOfBirth, LoverType? loverType, bool isHotSeat, bool isBoosted, int boostDay, int hotSeatDay)

        { 
            Gender = gender;
            PreferredGenders = preferredGenders;
            Location = location;
            Nationality = nationality;
            MaxDistance = maxDistance;
            Age = age;
            MinPreferredAge = minPreferredAge;
            MaxPreferredAge = maxPreferredAge;
            Bio = bio;
            DisplayStarSign = displayStarSign;
            IsArchived = isArchived;
            Photos = photos;
            Interests = interests;
            DateOfBirth = dateOfBirth;
            LoverType = loverType;
            IsHotSeat = isHotSeat;
            IsBoosted = isBoosted;
            BoostDay = boostDay;
            HotSeatDay = hotSeatDay;
        }
        public StarSign GetStarSign()
        {
            int month = DateOfBirth.Month;
            int day = DateOfBirth.Day;
            return month switch
            {
                1 => day <= 19 ? StarSign.CAPRICORN : StarSign.AQUARIUS,
                2 => day <= 18 ? StarSign.AQUARIUS : StarSign.PISCES,
                3 => day <= 20 ? StarSign.PISCES : StarSign.ARIES,
                4 => day <= 19 ? StarSign.ARIES : StarSign.TAURUS,
                5 => day <= 20 ? StarSign.TAURUS : StarSign.GEMINI,
                6 => day <= 20 ? StarSign.GEMINI : StarSign.CANCER,
                7 => day <= 22 ? StarSign.CANCER : StarSign.LEO,
                8 => day <= 22 ? StarSign.LEO : StarSign.VIRGO,
                9 => day <= 22 ? StarSign.VIRGO : StarSign.LIBRA,
                10 => day <= 22 ? StarSign.LIBRA : StarSign.SCORPIO,
                11 => day <= 21 ? StarSign.SCORPIO : StarSign.SAGITTARIUS,
                12 => day <= 21 ? StarSign.SAGITTARIUS : StarSign.CAPRICORN
            };
        }



    }
}
