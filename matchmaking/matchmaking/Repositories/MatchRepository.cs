using matchmaking.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace matchmaking.Repositories
{
    internal class MatchRepository
    {
        private readonly string _connectionString;

        public MatchRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private Match MapMatch(SqlDataReader reader)
        {
            int id = (int)reader["matchId"];
            int user1Id = (int)reader["user1Id"];
            int user2Id = (int)reader["user2Id"];

            return new Match(id, user1Id, user2Id);
        }

        public List<Match> GetAll()
        {
            const string query = @"
                SELECT matchId, user1Id, user2Id
                FROM Matches";

            List<Match> matches = new List<Match>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                matches.Add(MapMatch(reader));
            }

            return matches;
        }

        public void Add(Match match)
        {
            const string query = @"
                INSERT INTO Matches (user1Id, user2Id)
                VALUES (@user1Id, @user2Id);";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@user1Id", match.User1Id);
            command.Parameters.AddWithValue("@user2Id", match.User2Id);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public Match DeleteById(int matchId)
        {
            const string query = @"
                DELETE FROM Matches
                OUTPUT DELETED.matchId, DELETED.user1Id, DELETED.user2Id
                WHERE matchId = @matchId;";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@matchId", matchId);
            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapMatch(reader);
            }

            return null;
        }

        public Match FindById(int matchId)
        {
            const string query = @"
                SELECT matchId, user1Id, user2Id
                FROM Matches
                WHERE matchId = @matchId;";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@matchId", matchId);
            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapMatch(reader);
            }

            return null;
        }
    }
}
