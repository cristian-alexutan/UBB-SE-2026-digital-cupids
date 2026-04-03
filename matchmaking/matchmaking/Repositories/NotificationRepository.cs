using matchmaking.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace matchmaking.Repositories
{
    internal class NotificationRepository
    {
        private readonly string _connectionString;

        public NotificationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private Notification MapNotification(SqlDataReader reader)
        {
            int notificationId = (int)reader["notificationId"];
            int recipientId = (int)reader["recipientId"];
            int fromId = (int)reader["fromId"];
            string typeString = reader["type"].ToString()!;
            bool isRead = (bool)reader["isRead"];
            DateTime timestamp = (DateTime)reader["timestamp"];
            string title = (string)reader["title"];
            string description = (string)reader["description"];

            NotificationType type = Enum.Parse<NotificationType>(typeString);

            return new Notification(notificationId, recipientId, fromId, type, isRead, timestamp, title, description);
        }

        public List<Notification> GetAll()
        {
            const string query = @"
                SELECT notificationId, recipientId, fromId, [type], isRead, [timestamp], title, [description]
                FROM Notifications;";
            List<Notification> notifications = new List<Notification>();
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                notifications.Add(MapNotification(reader));
            }
            return notifications;
        }

        public void Add(Notification notification)
        {
            const string query = @"
                INSERT INTO Notifications (recipientId, fromId, [type], isRead, [timestamp], title, [description])
                VALUES (@recipientId, @fromId, @type, @isRead, @timestamp, @title, @description)";

            using SqlConnection dbConn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(query, dbConn);

            cmd.Parameters.AddWithValue("@recipientId", notification.RecipientId);
            cmd.Parameters.AddWithValue("@fromId", notification.FromId);
            cmd.Parameters.AddWithValue("@type", notification.Type.ToString());
            cmd.Parameters.AddWithValue("@isRead", notification.IsRead);
            cmd.Parameters.AddWithValue("@timestamp", notification.CreatedAt);
            cmd.Parameters.AddWithValue("@title", notification.Title);
            cmd.Parameters.AddWithValue("@description", notification.Description);

            dbConn.Open();
            cmd.ExecuteNonQuery();
        }

        public Notification DeleteById(int notificationId)
        {
            const string query = @"
                DELETE FROM Notifications
                OUTPUT DELETED.notificationId, DELETED.recipientId, DELETED.fromId, DELETED.[type], DELETED.isRead, DELETED.[timestamp], DELETED.title, DELETED.[description]
                WHERE notificationId = @notificationId;";

            using SqlConnection dbConn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(query, dbConn);

            cmd.Parameters.AddWithValue("@notificationId", notificationId);

            dbConn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapNotification(reader);
            }

            return null;
        }

        public Notification FindById(int notificationId)
        {
            const string query = @"
                SELECT notificationId, recipientId, fromId, [type], isRead, [timestamp], title, [description]
                FROM Notifications
                WHERE notificationId = @notificationId;";

            using SqlConnection dbConn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(query, dbConn);

            cmd.Parameters.AddWithValue("@notificationId", notificationId);

            dbConn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapNotification(reader);
            }

            return null;
        }

        public Notification MarkRead(int notificationId)
        {
            const string query = @"
                UPDATE Notifications
                SET isRead = @isRead
                OUTPUT INSERTED.notificationId, INSERTED.recipientId, INSERTED.fromId, INSERTED.[type], INSERTED.isRead, INSERTED.[timestamp], INSERTED.title, INSERTED.[description]
                WHERE notificationId = @notificationId;";

            using SqlConnection dbConn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(query, dbConn);

            cmd.Parameters.AddWithValue("@notificationId", notificationId);
            cmd.Parameters.AddWithValue("@isRead", true);

            dbConn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapNotification(reader);
            }

            return null;
        }
    }
}
