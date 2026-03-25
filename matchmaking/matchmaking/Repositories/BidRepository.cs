using System;
using System.Collections.Generic;
using System.Text;
using matchmaking.Domain;
using Microsoft.Data.SqlClient;
namespace matchmaking.Repositories
{
    internal class BidRepository
    {
        private readonly string _connectionString;
        public int BidDay { get; set; }

        public BidRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private Bid MapBid(SqlDataReader reader)
        {
            int id = (int)reader["bidId"];
            int userId = (int)reader["userId"];
            int bidSum = (int)reader["bidSum"];
            return new Bid(id, userId, bidSum);
        }

        public List<Bid> GetAll()
        {
            const string query = @"SELECT bidId, userId, bidSum FROM Bids";
            List<Bid> bids = new List<Bid>();
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                bids.Add(MapBid(reader));
            }
            return bids;
        }

        public void Add(Bid bid)
        {
            const string query = @"INSERT INTO Bids (userId, bidSum) VALUES (@userId, @bidSum);";
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@userId", bid.UserId);
            command.Parameters.AddWithValue("@bidSum", bid.BidSum);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public Bid DeleteById(int bidId)
        {
            const string query = @"DELETE FROM Bids OUTPUT DELETED.bidId, DELETED.userId, DELETED.BidSum WHERE bidId = @bidId";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@bidId", bidId);
            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) { 
                return MapBid(reader);
            }

            return null;
        }

        public void Clear(int newday)
        {
            const string query = @"DELETE FROM Bids";
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            command.ExecuteNonQuery();
            BidDay = newday;
        }
    }
}
