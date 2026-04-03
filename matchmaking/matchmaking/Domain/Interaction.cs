using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace matchmaking.Domain
{
    internal enum InteractionType
    {
        LIKE,
        SUPER_LIKE,
        PASS
    }
    internal class Interaction
    {
        public int InteractionId { get; }
        public int FromProfileId { get; }
        public int ToProfileId { get; }
        public InteractionType Type { get; }

        public Interaction(int fromProfileId, int toProfileId, InteractionType type)
        {
            FromProfileId = fromProfileId;
            ToProfileId = toProfileId;
            Type = type;
        }
        public Interaction(int interactionId, int fromProfileId, int toProfileId, InteractionType type)
        {
            InteractionId = interactionId;
            FromProfileId = fromProfileId;
            ToProfileId = toProfileId;
            Type = type;
        }
    }
}
