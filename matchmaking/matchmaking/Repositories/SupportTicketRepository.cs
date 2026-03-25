using matchmaking.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matchmaking.Repositories
{
    internal class SupportTicketRepository
    {
        private readonly string _connectionString;
        private List<SupportTicket> Tickets;

        public SupportTicketRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SupportTicket MapSupportTicket(SqlDataReader reader)
        {
            string email = (string)reader["email"];
            string partnerName = (string)reader["partnerName"];
            string marriageCertificatePath = (string)reader["certificateUrl"];
            string partnerPhotoPath = (string)reader["partnerPhotoUrl"];
            bool isResolved = (bool)reader["isResolved"];

            return new SupportTicket(email, partnerName, marriageCertificatePath, partnerPhotoPath, isResolved);
        }

        public List<SupportTicket> GetAll()
        {
            const string query = @"SELECT email,partnerName,certificateUrl,partnerPhotoUrl,isResolved FROM SupportTicket;";

            List<SupportTicket> tickets = new List<SupportTicket>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                tickets.Add(MapSupportTicket(reader));
            }

            return tickets;
        }

        public void Add(SupportTicket ticket)
        {
            const string query = @"INSERT INTO SupportTicket (email,partnerName,certificateUrl,partnerPhotoUrl,isResolved) 
                                    VALUES (@email,@partnerName,@certificateUrl,@partnerPhotoUrl,@isResolved);";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@email", ticket.Email);
            command.Parameters.AddWithValue("@partnerName", ticket.PartnerName);
            command.Parameters.AddWithValue("@certificateUrl", ticket.MarriageCertificatePath);
            command.Parameters.AddWithValue("@partnerPhotoUrl", ticket.PartnerPhotoPath);
            command.Parameters.AddWithValue("@isResolved", ticket.IsResolved);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public SupportTicket DeleteByEmail(string email) {
            const string query = @"DELETE FROM SupportTicket
                OUTPUT DELETED.email, DELETED.partnerName, DELETED.certificateUrl, DELETED.partnerPhotoUrl, DELETED.isResolved
                WHERE email = @email;";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@email", email);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapSupportTicket(reader);
            }

            return null;
        }

        public SupportTicket FindByEmail(string email)
        {
            const string query = @"SELECT email FROM SupportTicket WHERE email = @email;";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@email", email);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapSupportTicket(reader);
            }

            return null;
        }

        public void UpdateIsSolved(string email, bool isResolved)
        {
            const string query = @"UPDATE SupportTicket SET isResolved = @isResolved WHERE email = @email;";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@isResolved", isResolved);

            connection.Open();
            command.ExecuteNonQuery();
        }


    }
}
