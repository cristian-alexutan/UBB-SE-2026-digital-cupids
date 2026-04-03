using System;
using System.Collections.Generic;
using System.Text;
using matchmaking.Domain;
using matchmaking.Repositories;
using Windows.ApplicationModel.Activation;

namespace matchmaking.Services
{
    internal class MatchService
    {
        private readonly MatchRepository _matchRepository;

        public MatchService(MatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public void AddMatch(Match match)
        {
            _matchRepository.Add(match);
        }

        public Match DeleteById(int matchId)
        {
            return _matchRepository.DeleteById(matchId);
        }

        public Match FindById(int matchId)
        {
            return _matchRepository.FindById(matchId);
        }

        public List<Match> FindByUserId(int userId)
        {
            List<Match> matches = _matchRepository.GetAll();
            List<Match> filteredMatches = new List<Match>();

            foreach (Match match in matches)
            {
                if (match.User1Id == userId || match.User2Id == userId)
                {
                    filteredMatches.Add(match);
                }
            }
            return filteredMatches;
        }
    }
}
