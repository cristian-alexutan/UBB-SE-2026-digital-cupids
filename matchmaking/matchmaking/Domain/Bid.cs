using System;
using System.Collections.Generic;
using System.Text;

namespace matchmaking.Domain
{
    internal class Bid
    {
        public int BidId { get; }

        public int UserId { get; }

        public int BidSum { get; set; }

        public Bid(int bidId, int userId, int bidSum)
        {
            BidId = bidId;
            UserId = userId;
            BidSum = bidSum;
        }

        public Bid(int userId, int bidSum)
        {
            UserId = userId;
            BidSum = bidSum;
        }
    }
}
