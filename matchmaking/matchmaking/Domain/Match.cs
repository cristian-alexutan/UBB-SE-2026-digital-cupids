using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matchmaking.Domain
{
    internal class Match
    {
        public int MatchId { get; }
        public int User1Id { get; }
        public int User2Id { get; }

        public Match(int user1Id, int user2Id)
        {
            User1Id = user1Id;
            User2Id = user2Id;
        }
        public Match(int matchId, int user1Id, int user2Id)
        {
            MatchId = matchId;
            User1Id = user1Id;
            User2Id = user2Id;
        }
    }
}
