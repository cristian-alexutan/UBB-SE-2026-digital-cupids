using matchmaking.Domain;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matchmaking.Repositories
{
    internal class DatingAdminRepository
    {
        private readonly string _connectionString;
        private List<DatingAdmin> Admins;

        public DatingAdminRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private DatingAdmin MapDatingAdmin(SqlDataReader reader)
        {
            int id = (int) reader["userId"];
            return new DatingAdmin(id);
        }

        public List<DatingAdmin> GetAll()
        {
            const string query = @"SELECT userId FROM DatingAdmin;";

            List<DatingAdmin> admins = new List<DatingAdmin>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                admins.Add(MapDatingAdmin(reader));
            }

            return admins;
        }

        public DatingAdmin FindById (int userId)
        {
            const string query = @"SELECT userId FROM DatingAdmin;";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapDatingAdmin(reader);
            }

            return null;
        }
    }
}
