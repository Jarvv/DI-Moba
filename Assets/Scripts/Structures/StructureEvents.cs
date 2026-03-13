using Core.Teams;

namespace Structures
{
    public struct StructureDestroyedEvent
    {
        public Structure Structure;
        public Team? KillerTeam;
        public int GoldReward;
    }
}