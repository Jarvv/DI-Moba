using Core.Teams;

namespace Game.GameState
{
    public struct GameOverEvent
    {
        public Team DestroyedNexusTeam;
        public Team? WinnerTeam;
    }
}
