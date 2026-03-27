using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matchmaking.Utils
{
    internal class UserData
    {
        public int Id { get; }
        public string Username { get; set; }
        public string Email { get; }
        public string Phone { get; }
        public string AvatarUrl { get; }
        public string Bio { get;}
        public DateTime Birthdate { get; }

        public UserData(int id, string username, string email, string phone, string avatarUrl, string bio, DateTime birthdate)
        {
            Id = id;
            Username = username;
            Email = email;
            Phone = phone;
            AvatarUrl = avatarUrl;
            Bio = bio;
            Birthdate = birthdate;
        }
    }
}
