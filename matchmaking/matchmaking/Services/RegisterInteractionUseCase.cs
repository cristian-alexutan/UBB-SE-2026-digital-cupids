using System;
using System.Collections.Generic;
using System.Text;
using matchmaking.Domain;
using matchmaking.Repositories;

namespace matchmaking.Services
{
    public class RegisterInteractionUseCase
    {
        private readonly InteractionService _interactionService;
        private readonly MatchService _matchService;
        private readonly NotificationService _notificationService;
        private readonly ProfileRepository _profileRepository;

        public RegisterInteractionUseCase(
            InteractionService interactionService,
            MatchService matchService,
            NotificationService notificationService,
            ProfileRepository profileRepository)
        {
            _interactionService = interactionService;
            _matchService = matchService;
            _notificationService = notificationService;
            _profileRepository = profileRepository;
        }

        public bool RegisterInteraction(Interaction interaction)
        {
            _interactionService.LogInteraction(interaction);

            if (interaction.Type == InteractionType.SUPER_LIKE)
            {
                string username = ResolveUsername(interaction.FromProfileId);
                Notification superLikeNotification = new Notification(
                    interaction.ToProfileId,
                    interaction.FromProfileId,
                    NotificationType.SUPER_LIKE,
                    $"Super-Like!",
                    $"{username} sent you a super like."
                );
                _notificationService.AddNotification(superLikeNotification);
            }

            if (DetectsMatch(interaction))
            {
                Match match = new Match(interaction.FromProfileId, interaction.ToProfileId);
                _matchService.AddMatch(match);

                string fromUserName = ResolveUsername(interaction.FromProfileId);
                string toUserName = ResolveUsername(interaction.ToProfileId);

                Notification notificationForToUser = new Notification(
                    interaction.ToProfileId,
                    interaction.FromProfileId,
                    NotificationType.MATCH,
                    $"It's a match!",
                    $"You matched with {fromUserName}"
                );

                Notification notificationForFromUser = new Notification(
                    interaction.FromProfileId,
                    interaction.ToProfileId,
                    NotificationType.MATCH,
                    $"It's a match!",
                    $"You matched with {toUserName}"
                );

                _notificationService.AddNotification(notificationForToUser);
                _notificationService.AddNotification(notificationForFromUser);

                return true;
            }
            return false;
        }

        private bool DetectsMatch(Interaction interaction)
        {
            if (!IsPositiveInteraction(interaction.Type))
            {
                return false;
            }

            bool senderLikesReceiver = HasPositiveInteraction(interaction.FromProfileId, interaction.ToProfileId);
            bool receiverLikesSender = HasPositiveInteraction(interaction.ToProfileId, interaction.FromProfileId);

            if (!senderLikesReceiver || !receiverLikesSender)
            {
                return false;
            }

            List<Match> existingMatches = _matchService.FindByUserId(interaction.FromProfileId);
            foreach (Match existingMatch in existingMatches)
            {
                bool samePair =
                    (existingMatch.User1Id == interaction.FromProfileId && existingMatch.User2Id == interaction.ToProfileId) ||
                    (existingMatch.User1Id == interaction.ToProfileId && existingMatch.User2Id == interaction.FromProfileId);

                if (samePair)
                {
                    return false;
                }
            }

            return true;
        }

        private bool HasPositiveInteraction(int fromProfileId, int toProfileId)
        {
            List<Interaction> sentInteractions = _interactionService.FindBySenderId(fromProfileId);

            foreach (Interaction sentInteraction in sentInteractions)
            {
                if (sentInteraction.ToProfileId == toProfileId && IsPositiveInteraction(sentInteraction.Type))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsPositiveInteraction(InteractionType type)
        {
            return type == InteractionType.LIKE || type == InteractionType.SUPER_LIKE;
        }

        private string ResolveUsername(int profileId)
        {
            try
            {
                DatingProfile? profile = _profileRepository.FindById(profileId);
                if (profile != null)
                {
                    return profile.Name;
                }

                return "Someone";
            }
            catch
            {
                return "Someone";
            }
        }

    }
}
