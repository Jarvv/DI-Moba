using Core.Teams;

namespace Creeps
{
    public struct CreepDiedEvent
    {
        public Creep Creep;
        public Team? KillerTeam;
        public int GoldReward;
    }
}
