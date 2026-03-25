using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using matchmaking.Domain;

namespace matchmaking.Repositories
{
    internal class InteractionRepository
    {
        private readonly string _connectionString;

        public InteractionRepository(string connectionString)
        {
            _connectionString = connectionString;

        }

        private Interaction MapInteraction(SqlDataReader reader)
        {
            int id = (int)reader["interactionId"];
            int fromId = (int)reader["fromProfileId"];
            int toId = (int)reader["toProfileId"];
            string typeString = reader["type"].ToString()!;

            InteractionType type = Enum.Parse<InteractionType>(typeString);

            return new Interaction(id, fromId, toId, type);
        }

        public List<Interaction> GetAll()
        {
            const string query = @"
                SELECT interactionId, fromProfileId, toProfileId, [type]
                FROM Interactions;";

            List<Interaction> interactions = new List<Interaction>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                interactions.Add(MapInteraction(reader));
            }

            return interactions;
        }
        public void Add(Interaction interaction)
        {
            const string query = @"
                INSERT INTO Interactions (fromProfileId, toProfileId, [type])
                VALUES (@fromProfileId, @toProfileId, @type);";

            SqlConnection dbConn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(query, dbConn);

            cmd.Parameters.AddWithValue("@fromProfileId", interaction.FromProfileId);
            cmd.Parameters.AddWithValue("@toProfileId", interaction.ToProfileId);
            cmd.Parameters.AddWithValue("@type", interaction.Type.ToString());

            dbConn.Open();
            cmd.ExecuteNonQuery();
        }

        public Interaction? DeleteById(int interactionId)
        {
            const string query = @"
                DELETE FROM Interactions
                OUTPUT DELETED.interactionId, DELETED.fromProfileId, DELETED.toProfileId, DELETED.[type]
                WHERE interactionId = @interactionId;";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@interactionId", interactionId);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapInteraction(reader);
            }

            return null;
        }
    }
}
