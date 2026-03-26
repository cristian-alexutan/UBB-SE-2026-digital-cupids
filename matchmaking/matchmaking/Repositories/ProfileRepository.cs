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
        private List<DatingProfile> Profiles;

        public ProfileRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        private DatingProfile MapProfile(SqlDataReader reader)
        {
            int userId = (int)reader["userId"];
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

            return new DatingProfile(userId, gender, new List<Gender>(), location, nationality,
                maxDistance, age, minPrefAge, maxPrefAge, bio, displayStarSign, isArchived,
                new List<Photo>(), new List<string>(), dateOfBirth, loverType, isHotSeat,
                isBoosted, boostDay, hotSeatDay);
        }
        public List<DatingProfile> GetAll()
        {
            const string query = @"SELECT userId, gender, location, nationality, maxDistance, age,minPrefAge, maxPrefAge,
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

            return profiles;
        }
        public void Add(DatingProfile profile)
        {
            const string query = @"
                INSERT INTO Profiles (gender, location, nationality, maxDistance, age, minPrefAge, maxPrefAge, bio, displayStarSign, isArchived,
                    dateOfBirth, loverType, isHotSeat, boost, boostDay, hotSeatDay)
                    VALUES (@gender, @location, @nationality, @maxDistance, @age,@minPrefAge, @maxPrefAge, @bio, @displayStarSign, @isArchived,
                    @dateOfBirth, @loverType, @isHotSeat, @boost, @boostDay, @hotSeatDay);";

            using SqlConnection connection = new SqlConnection(_connectionString);
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

            connection.Open();
            command.ExecuteNonQuery();
        }
        public DatingProfile? DeleteById(int id)
        {
            const string query = @"
                DELETE FROM Profiles
                OUTPUT DELETED.userId, DELETED.gender, DELETED.location, DELETED.nationality,DELETED.maxDistance, DELETED.age, DELETED.minPrefAge, DELETED.maxPrefAge,
                ELETED.bio, DELETED.displayStarSign, DELETED.isArchived, DELETED.dateOfBirth,DELETED.loverType, DELETED.isHotSeat, DELETED.boost, DELETED.boostDay, DELETED.hotSeatDay
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
                SELECT userId, gender, location, nationality, maxDistance, age, minPrefAge, maxPrefAge, bio, displayStarSign, isArchived,
                dateOfBirth, loverType, isHotSeat, boost, boostDay, hotSeatDay
                FROM Profiles
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
        public DatingProfile? Update(DatingProfile profile)
        {
            const string query = @"
                UPDATE Profiles
                SET gender = @gender, location = @location, nationality = @nationality, maxDistance = @maxDistance, age = @age, minPrefAge = @minPrefAge,
                maxPrefAge = @maxPrefAge, bio = @bio, displayStarSign = @displayStarSign, isArchived = @isArchived, dateOfBirth = @dateOfBirth, loverType = @loverType,
                isHotSeat = @isHotSeat, boost = @boost, boostDay = @boostDay, hotSeatDay = @hotSeatDay
                WHERE userId = @userId;";

            using SqlConnection connection = new SqlConnection(_connectionString);
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

            connection.Open();
            command.ExecuteNonQuery();

            return profile;
        }

    }
}
