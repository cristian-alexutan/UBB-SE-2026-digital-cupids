using matchmaking.Domain;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace matchmaking.Repositories
{
    internal class PhotoRepository
    {
        private readonly string _connectionString;

        public PhotoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private Photo MapPhoto(SqlDataReader reader)
        {
            int id = (int)reader["photoId"];
            int userId = (int)reader["userId"];
            string location = (string)reader["location"];
            int profileOrderIndex = (int)reader["profileOrderIndex"];

            return new Photo(id, userId, location, profileOrderIndex);
        }

        public List<Photo> GetAll()
        {
            const string query = @"
                SELECT photoId, userId, [location], profileOrderIndex 
                FROM Photos 
                ORDER BY userId, profileOrderIndex ASC";

            List<Photo> photos = new List<Photo>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                photos.Add(MapPhoto(reader));
            }

            return photos;
        }

        public void Add(Photo photo)
        {
            const string query = @"
                INSERT INTO Photos (userId, [location], profileOrderIndex) 
                VALUES (@userId, @location, @profileOrderIndex)";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@userId", photo.UserId);
            command.Parameters.AddWithValue("@location", photo.Location);
            command.Parameters.AddWithValue("@profileOrderIndex", photo.ProfileOrderIndex);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public Photo? DeleteById(int photoId)
        {
            const string query = @"
                DELETE FROM Photos 
                OUTPUT DELETED.photoId, DELETED.userId, DELETED.[location], DELETED.profileOrderIndex
                WHERE photoId = @photoId;";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@photoId", photoId);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapPhoto(reader);
            }

            return null;
        }

        public Photo? FindById(int photoId)
        {
            const string query = @"
                SELECT photoId, userId, [location], profileOrderIndex 
                FROM Photos 
                WHERE photoId = @photoId";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@photoId", photoId);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapPhoto(reader);
            }

            return null;
        }

        public void Update(Photo photo)
        {
            const string query = @"
                UPDATE Photos
                SET [location] = @location, profileOrderIndex = @profileOrderIndex
                WHERE photoId = @photoId";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@location", photo.Location);
            command.Parameters.AddWithValue("@profileOrderIndex", photo.ProfileOrderIndex);
            command.Parameters.AddWithValue("@photoId", photo.PhotoId);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
