using matchmaking.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using matchmaking.Domain;
using System.Diagnostics;
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
            Debug.WriteLine($"BidService: reached addbid with UserId: {bid.UserId}, BidSum: {bid.BidSum}");
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
            if (bid.BidSum < highestBidSum + 10)
            {
                throw new Exception($"Bid sum must be at least 10 higher than the current highest bid of {highestBidSum}.");
            }
            Debug.WriteLine($"BidService: Adding bid with UserId: {bid.UserId}, BidSum: {bid.BidSum} to repository., calling repo function");
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
            foreach (var b in bids)
            {
                if (b.BidSum > highestBidSum)
                {
                    highestBidSum = b.BidSum;
                }
            }
            return highestBidSum;
        }

        public int getHighestBidderId()
        {
            int today = DateTime.Today.Day;
            if (BidRepo.BidDay != today)
            {
                BidRepo.Clear(today);
            }
            List<Bid> bids = BidRepo.GetAll();
            int highestBidSum = 0;
            int highestBidderId = 0;
            foreach (var b in bids)
            {
                if (b.BidSum > highestBidSum)
                {
                    highestBidSum = b.BidSum;
                    highestBidderId = b.UserId;
                }
            }
            return highestBidderId;
        }
    }
}
