using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using matchmaking.Domain;

namespace matchmaking.Repositories
{
    internal class ProfileRepository
    {
        private readonly string _connectionString;

        public ProfileRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        private DatingProfile MapProfile(SqlDataReader reader)
        {
            int userId = (int)reader["userId"];
            string name = reader["name"].ToString()!;
            Gender gender = Enum.Parse<Gender>(reader["gender"].ToString()!);
            string location = reader["location"].ToString()!;
            string nationality = reader["nationality"].ToString()!;
            int maxDistance = (int)reader["maxDistance"];
            int age = (int)reader["age"];
            int minPrefAge = (int)reader["minPrefAge"];
            int maxPrefAge = (int)reader["maxPrefAge"];
            string bio = reader["bio"].ToString()!;
            bool displayStarSign = (bool)reader["displayStarSign"];
            bool isArchived = reader["isArchived"] != DBNull.Value && (bool)reader["isArchived"];
            DateTime dateOfBirth = (DateTime)reader["dateOfBirth"];
            LoverType? loverType = reader["loverType"] != DBNull.Value
                ? Enum.Parse<LoverType>(reader["loverType"].ToString()!)
                : null;
            bool isHotSeat = reader["isHotSeat"] != DBNull.Value && (bool)reader["isHotSeat"];
            bool isBoosted = reader["boost"] != DBNull.Value && (bool)reader["boost"];
            int boostDay = reader["boostDay"] != DBNull.Value ? (int)reader["boostDay"] : 0;
            int hotSeatDay = reader["hotSeatDay"] != DBNull.Value ? (int)reader["hotSeatDay"] : 0;

            return new DatingProfile(userId, name, gender, new List<Gender>(), location, nationality,
                maxDistance, age, minPrefAge, maxPrefAge, bio, displayStarSign, isArchived,
                new List<Photo>(), new List<string>(), dateOfBirth, loverType, isHotSeat,
                isBoosted, boostDay, hotSeatDay);
        }
        private List<Gender> GetPreferredGenders(int userId, SqlConnection connection)
        {
            const string query = "SELECT gender FROM ProfilePreferences WHERE userId = @userId";
            List<Gender> genders = new List<Gender>();

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                genders.Add(Enum.Parse<Gender>(reader["gender"].ToString()!));
            }

            return genders;
        }

        private List<Photo> GetPhotos(int userId, SqlConnection connection)
        {
            const string query = "SELECT photoId, userId, location, profileOrderIndex FROM Photos WHERE userId = @userId";
            List<Photo> photos = new List<Photo>();

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                photos.Add(new Photo(
                    (int)reader["photoId"],
                    (int)reader["userId"],
                    reader["location"].ToString()!,
                    (int)reader["profileOrderIndex"]
                ));
            }

            return photos;
        }
        private void PopulateLists(DatingProfile profile, SqlConnection connection)
        {
            profile.PreferredGenders = GetPreferredGenders(profile.UserId, connection);
            profile.Photos = GetPhotos(profile.UserId, connection);
            profile.Interests = GetInterests(profile.UserId, connection);
        }
        private List<string> GetInterests(int userId, SqlConnection connection)
        {
            const string query = "SELECT interest FROM ProfileInterests WHERE userId = @userId";
            List<string> interests = new List<string>();

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                interests.Add(reader["interest"].ToString()!);
            }

            return interests;
        }
        public List<DatingProfile> GetAll()
        {
            const string query = @"SELECT userId, name, gender, location, nationality, maxDistance, age,minPrefAge, maxPrefAge,
                                   bio, displayStarSign, isArchived, dateOfBirth, loverType, isHotSeat, boost, boostDay, hotSeatDay
                                   FROM Profiles;";

            List<DatingProfile> profiles = new List<DatingProfile>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                profiles.Add(MapProfile(reader));
            }
            reader.Close();
            foreach (DatingProfile profile in profiles)
            {
                PopulateLists(profile, connection);
            }

            return profiles;
        }
        public void Add(DatingProfile profile)
        {
         

            const string query = @"
            INSERT INTO Profiles (gender, location, nationality, maxDistance, age,
                minPrefAge, maxPrefAge, bio, displayStarSign, isArchived,
                dateOfBirth, loverType, isHotSeat, boost, boostDay, hotSeatDay)
            VALUES (@gender, @location, @nationality, @maxDistance, @age,
                @minPrefAge, @maxPrefAge, @bio, @displayStarSign, @isArchived,
                @dateOfBirth, @loverType, @isHotSeat, @boost, @boostDay, @hotSeatDay);
            SELECT SCOPE_IDENTITY();";

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@gender", profile.Gender.ToString());
            command.Parameters.AddWithValue("@location", profile.Location);
            command.Parameters.AddWithValue("@nationality", profile.Nationality);
            command.Parameters.AddWithValue("@maxDistance", profile.MaxDistance);
            command.Parameters.AddWithValue("@age", profile.Age);
            command.Parameters.AddWithValue("@minPrefAge", profile.MinPreferredAge);
            command.Parameters.AddWithValue("@maxPrefAge", profile.MaxPreferredAge);
            command.Parameters.AddWithValue("@bio", profile.Bio);
            command.Parameters.AddWithValue("@displayStarSign", profile.DisplayStarSign);
            command.Parameters.AddWithValue("@isArchived", profile.IsArchived);
            command.Parameters.AddWithValue("@dateOfBirth", profile.DateOfBirth);
            command.Parameters.AddWithValue("@loverType", profile.LoverType.HasValue ? profile.LoverType.ToString() : DBNull.Value);
            command.Parameters.AddWithValue("@isHotSeat", profile.IsHotSeat);
            command.Parameters.AddWithValue("@boost", profile.IsBoosted);
            command.Parameters.AddWithValue("@boostDay", profile.BoostDay);
            command.Parameters.AddWithValue("@hotSeatDay", profile.HotSeatDay);

            int newUserId = Convert.ToInt32(command.ExecuteScalar());

            foreach (Gender gender in profile.PreferredGenders)
            {
                const string genderQuery = "INSERT INTO ProfilePreferences (userId, gender) VALUES (@userId, @gender)";
                using SqlCommand genderCmd = new SqlCommand(genderQuery, connection);
                genderCmd.Parameters.AddWithValue("@userId", newUserId);
                genderCmd.Parameters.AddWithValue("@gender", gender.ToString());
                genderCmd.ExecuteNonQuery();
            }

            foreach (Photo photo in profile.Photos)
            {
                const string photoQuery = "INSERT INTO Photos (userId, location, profileOrderIndex) VALUES (@userId, @location, @profileOrderIndex)";
                using SqlCommand photoCmd = new SqlCommand(photoQuery, connection);
                photoCmd.Parameters.AddWithValue("@userId", newUserId);
                photoCmd.Parameters.AddWithValue("@location", photo.Location);
                photoCmd.Parameters.AddWithValue("@profileOrderIndex", photo.ProfileOrderIndex);
                photoCmd.ExecuteNonQuery();
            }

            foreach (string interest in profile.Interests)
            {
                const string interestQuery = "INSERT INTO ProfileInterests (userId, interest) VALUES (@userId, @interest)";
                using SqlCommand interestCmd = new SqlCommand(interestQuery, connection);
                interestCmd.Parameters.AddWithValue("@userId", newUserId);
                interestCmd.Parameters.AddWithValue("@interest", interest);
                interestCmd.ExecuteNonQuery();
            }
        }
        public DatingProfile? DeleteById(int id)
        {
            const string query = @"
                DELETE FROM Profiles
                OUTPUT DELETED.userId, DELETED.name, DELETED.gender, DELETED.location, DELETED.nationality,DELETED.maxDistance, DELETED.age, DELETED.minPrefAge, DELETED.maxPrefAge,
                DELETED.bio, DELETED.displayStarSign, DELETED.isArchived, DELETED.dateOfBirth,DELETED.loverType, DELETED.isHotSeat, DELETED.boost, DELETED.boostDay, DELETED.hotSeatDay
                WHERE userId = @userId;";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@userId", id);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
                return MapProfile(reader);

            return null;
        }

        public DatingProfile? FindById(int id)
        {
            const string query = @"
                SELECT userId, name, gender, location, nationality, maxDistance, age, minPrefAge, maxPrefAge, bio, displayStarSign, isArchived,
                dateOfBirth, loverType, isHotSeat, boost, boostDay, hotSeatDay
                FROM Profiles
                WHERE userId = @userId;";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@userId", id);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                DatingProfile profile = MapProfile(reader);
                reader.Close();                          
                PopulateLists(profile, connection);      
                return profile;
            }
                

            return null;
        }
        public DatingProfile? Update(DatingProfile profile)
        {   
            DatingProfile? existing = FindById(profile.UserId);
            if (existing != null) return null;


            const string query = @"
                UPDATE Profiles
                SET gender = @gender, location = @location, nationality = @nationality,
                    maxDistance = @maxDistance, age = @age, minPrefAge = @minPrefAge,
                    maxPrefAge = @maxPrefAge, bio = @bio, displayStarSign = @displayStarSign,
                    isArchived = @isArchived, dateOfBirth = @dateOfBirth, loverType = @loverType,
                    isHotSeat = @isHotSeat, boost = @boost, boostDay = @boostDay, hotSeatDay = @hotSeatDay
                WHERE userId = @userId;";

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", profile.UserId);
            command.Parameters.AddWithValue("@gender", profile.Gender.ToString());
            command.Parameters.AddWithValue("@location", profile.Location);
            command.Parameters.AddWithValue("@nationality", profile.Nationality);
            command.Parameters.AddWithValue("@maxDistance", profile.MaxDistance);
            command.Parameters.AddWithValue("@age", profile.Age);
            command.Parameters.AddWithValue("@minPrefAge", profile.MinPreferredAge);
            command.Parameters.AddWithValue("@maxPrefAge", profile.MaxPreferredAge);
            command.Parameters.AddWithValue("@bio", profile.Bio);
            command.Parameters.AddWithValue("@displayStarSign", profile.DisplayStarSign);
            command.Parameters.AddWithValue("@isArchived", profile.IsArchived);
            command.Parameters.AddWithValue("@dateOfBirth", profile.DateOfBirth);
            command.Parameters.AddWithValue("@loverType", profile.LoverType.HasValue ? profile.LoverType.ToString() : DBNull.Value);
            command.Parameters.AddWithValue("@isHotSeat", profile.IsHotSeat);
            command.Parameters.AddWithValue("@boost", profile.IsBoosted);
            command.Parameters.AddWithValue("@boostDay", profile.BoostDay);
            command.Parameters.AddWithValue("@hotSeatDay", profile.HotSeatDay);
            command.ExecuteNonQuery();

            
            bool gendersChanged = !existing.PreferredGenders.SequenceEqual(profile.PreferredGenders);
            if (gendersChanged)
            {
                const string deleteGenders = "DELETE FROM ProfilePreferences WHERE userId = @userId";
                using SqlCommand deleteGendersCmd = new SqlCommand(deleteGenders, connection);
                deleteGendersCmd.Parameters.AddWithValue("@userId", profile.UserId);
                deleteGendersCmd.ExecuteNonQuery();

                foreach (Gender gender in profile.PreferredGenders)
                {
                    const string insertGender = "INSERT INTO ProfilePreferences (userId, gender) VALUES (@userId, @gender)";
                    using SqlCommand insertGenderCmd = new SqlCommand(insertGender, connection);
                    insertGenderCmd.Parameters.AddWithValue("@userId", profile.UserId);
                    insertGenderCmd.Parameters.AddWithValue("@gender", gender.ToString());
                    insertGenderCmd.ExecuteNonQuery();
                }
            }

            bool interestsChanged = !existing.Interests.SequenceEqual(profile.Interests);
            if (interestsChanged)
            {
                const string deleteInterests = "DELETE FROM ProfileInterests WHERE userId = @userId";
                using SqlCommand deleteInterestsCmd = new SqlCommand(deleteInterests, connection);
                deleteInterestsCmd.Parameters.AddWithValue("@userId", profile.UserId);
                deleteInterestsCmd.ExecuteNonQuery();

                foreach (string interest in profile.Interests)
                {
                    const string insertInterest = "INSERT INTO ProfileInterests (userId, interest) VALUES (@userId, @interest)";
                    using SqlCommand insertInterestCmd = new SqlCommand(insertInterest, connection);
                    insertInterestCmd.Parameters.AddWithValue("@userId", profile.UserId);
                    insertInterestCmd.Parameters.AddWithValue("@interest", interest);
                    insertInterestCmd.ExecuteNonQuery();
                }
            }
           
                const string deletePhotos = "DELETE FROM Photos WHERE userId = @userId";
                using SqlCommand deletePhotosCmd = new SqlCommand(deletePhotos, connection);
                deletePhotosCmd.Parameters.AddWithValue("@userId", profile.UserId);
                deletePhotosCmd.ExecuteNonQuery();

                foreach (Photo photo in profile.Photos)
                {
                    const string insertPhoto = "INSERT INTO Photos (userId, location, profileOrderIndex) VALUES (@userId, @location, @profileOrderIndex)";
                    using SqlCommand insertPhotoCmd = new SqlCommand(insertPhoto, connection);
                    insertPhotoCmd.Parameters.AddWithValue("@userId", profile.UserId);
                    insertPhotoCmd.Parameters.AddWithValue("@location", photo.Location);
                    insertPhotoCmd.Parameters.AddWithValue("@profileOrderIndex", photo.ProfileOrderIndex);
                    insertPhotoCmd.ExecuteNonQuery();
                }
            

            return profile;
        }

    }
}
