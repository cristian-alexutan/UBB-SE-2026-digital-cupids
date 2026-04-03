using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matchmaking.Domain
{
    internal class Photo
    {
        public int PhotoId { get; set; }
        public int UserId { get; set; }
        public string? Location { get; set; }
        public int ProfileOrderIndex { get; set; }

        public Photo() { }

        public Photo(int userId, string location, int profileOrderIndex)
        {
            UserId = userId;
            Location = location;
            ProfileOrderIndex = profileOrderIndex;
        }
        public Photo(int photoId, int userId, string location, int profileOrderIndex)
        {
            PhotoId = photoId;
            UserId = userId;
            Location = location;
            ProfileOrderIndex = profileOrderIndex;
        }
    }
}
