using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matchmaking.Domain
{
    internal class DatingAdmin
    {
        public int UserId { get; set; }

        public DatingAdmin(int userId) {
            UserId = userId;
        }
    }
}
