using matchmaking.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using matchmaking.Domain;
namespace matchmaking.Services
{
    internal class BidService
    {
        private BidRepository BidRepo;

        public BidService(BidRepository bidRepo)
        {
            BidRepo = bidRepo;
        }

        public void AddBid(Bid bid)
        {
            int today = DateTime.Today.Day;
            if (BidRepo.BidDay != today)
            {
                BidRepo.Clear(today);
            }
            if (bid.BidSum < 50)
            {
                throw new Exception("Bid sum must be at least 50.");
            }
            int highestBidSum = getHighestBid();
            if(bid.BidSum < highestBidSum + 10)
            {
                throw new Exception($"Bid sum must be at least 10 higher than the current highest bid of {highestBidSum}.");
            }
            BidRepo.Add(bid);
        }

        public int getHighestBid()
        {
            int today = DateTime.Today.Day;
            if (BidRepo.BidDay != today)
            {
                BidRepo.Clear(today);
            }
            List<Bid> bids = BidRepo.GetAll();
            int highestBidSum = 0;
            foreach(var b in bids) {
                if (b.BidSum > highestBidSum)
                {
                    highestBidSum = b.BidSum;
                }
            }
            return highestBidSum;
        }
    }
}
