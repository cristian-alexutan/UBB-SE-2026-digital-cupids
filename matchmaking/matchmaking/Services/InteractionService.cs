using System;
using System.Collections.Generic;
using System.Text;
using matchmaking.Domain;
using matchmaking.Repositories;

namespace matchmaking.Services
{
    internal class InteractionService
    {
        private readonly InteractionRepository _interactionRepository;

        public InteractionService(InteractionRepository interactionRepository)
        {
            _interactionRepository = interactionRepository;
        }

        public void LogInteraction(Interaction interaction)
        {
            _interactionRepository.Add(interaction);
        }

        public Interaction? FindById(int interactionId)
        {
            return _interactionRepository.FindById(interactionId);
        }

        public List<Interaction> FindBySenderId(int senderId)
        {
            List<Interaction> interactions = _interactionRepository.GetAll();
            List<Interaction> filteredInteractions = new List<Interaction>();

            foreach (Interaction interaction in interactions)
            {
                if(interaction.FromProfileId == senderId)
                {
                    filteredInteractions.Add(interaction);
                }
            }
            return filteredInteractions;
        }

        public List<Interaction> FindByReceiverId(int receiverId)
        {
            List<Interaction> interactions = _interactionRepository.GetAll();
            List<Interaction> filteredInteractions = new List<Interaction>();
            foreach (Interaction interaction in interactions)
            {
                if (interaction.ToProfileId == receiverId)
                {
                    filteredInteractions.Add(interaction);
                }
            }
            return filteredInteractions;
        }
    }
}
